using asp_net_db.Data;
using asp_net_db.Models;
using asp_net_db.Models.Results;
using asp_net_db.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Renci.SshNet;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static asp_net_db.Models.Problem;

namespace asp_net_db.Services
{
    public class ServerService
    {
        private readonly ApplicationContext _context;

        public ServerService(ApplicationContext context)
        {
            _context = context;
        }
   
        public async Task<ErrorResult> PingSSH(Server server)
        {
            if (!server.UseSSH)
            {
                return new ErrorResult(403, $"SSH не подключен к серверу");
            }

            try
            {
                using (var client = new SshClient(server.HostnameSSH, server.PortSSH.Value, server.UsernameSSH, server.PasswordSSH))
                {
                    client.Connect();
                    // client.RunCommand("etc/init.d/networking restart");
                    client.Disconnect();
                }
                return null;
            }
            catch(Exception ex)
            {
                return new ErrorResult(404, $"Неизвестная ошибка: {ex.Message}");
            }   
        }

        public async Task<ErrorResult> PingServer(Server server)
        {
            var connectionString = $"Host={server.Host};Port={server.Port};Database={server.DbName};Username={server.Username};Password={server.Password}";
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                }

                return null;
            }
            catch (NpgsqlException ex)
            {
                return new ErrorResult(503, "Сервер недоступен для подключения");
            }
            catch (SocketException ex)
            {
                return new ErrorResult(503, "Этот хост неизвестен");
            }
            catch (Exception ex)
            {
                return new ErrorResult(404, $"Неизвестная ошибка: {ex.Message}");
            }
        }

        public async Task<ServerStats> GetServerMetrics(Server server)
        {
            var connectionString = $"Host={server.Host};Port={server.Port};Database={server.DbName};Username={server.Username};Password={server.Password}";

            ServerStats stats = new()
            {
                ServerId = server.Id,
                ProcessorPercentLoading = await GetProccessorPercentLoad(server),
                DatabaseSize = await GetDatabaseSize(connectionString),
                ConnectionInfo = await GetConnections(connectionString),
                LongIdleConnections = await GetIdleConnections(connectionString),
            };

            return stats;
        }

        public async Task<List<Problem>> FindProblems(Server server)
        {
            var connectionString = $"Host={server.Host};Port={server.Port};Database={server.DbName};Username={server.Username};Password={server.Password}";
            List<Problem> problems = new List<Problem>();

            // получение результатов
            var processorPercentLoading = await GetProccessorPercentLoad(server);
            var databaseSize = await GetDatabaseSize(connectionString);
            var needSize = (server.AllocatedSpace * server.Settings.DatabaseSizePercent) / 100;
            var connections = await GetConnections(connectionString);
            var stats = await _context.ServerStats.OrderBy(x => x.WritedAt).Include(x => x.ConnectionInfo).Include(x => x.LongIdleConnections).Where(x => x.ServerId == server.Id).Take(10).ToListAsync();

            /* конвейер ошибок */
            if (await PingServer(server) != null)
            {
                var problem = new Problem
                {
                    Alert = AlertType.Error.ToString(),
                    Type = "error_connection",
                    Text = $"Сервер недоступен к подключению",
                };

                return problems;
            }

            // обработка загрузки процессора
            if (processorPercentLoading >= server.Settings.MaxLoadingProcessor)
            {
                var problem = new Problem
                {
                    Alert = AlertType.Warning.ToString(),
                    Type = "processor_loaded",
                    Text = $"Процессор сервера загружен на {processorPercentLoading} процентов",
                };

                problems.Add(problem);
            }

            // обработка занятого пространства
            if (ConverterUtility.GetMegabytes(databaseSize) >= needSize)
            {
                var problem = new Problem
                {
                    Alert = AlertType.Error.ToString(),
                    Type = "free_space_running_out",
                    Text = $"Выделенное под базу место заполнено на {processorPercentLoading} процентов",
                };

                problems.Add(problem);
            }

            // 
            var sizeProc = server.Settings.MaxLoadingProcessor * 0.1;
            var sizeDiff = ConverterUtility.GetMegabytes(stats[0].DatabaseSize) - ConverterUtility.GetMegabytes(stats[stats.Count - 1].DatabaseSize);

            if (sizeDiff >= sizeProc)
            {
                var problem = new Problem
                {
                    Alert = AlertType.Error.ToString(),
                    Type = "free_space_running_out",
                    Text = $"Занятое место под базу данных резко выросло на {(sizeDiff * 100)/ server.Settings.MaxLoadingProcessor} процентов",
                };

                problems.Add(problem);
            }

            // обработка процента загруженных коннекшенов
            if (server.Settings.MaxConnectionsPercent <= connections.ConnectionsUtilization)
            {
                var problem = new Problem
                {
                    Alert = AlertType.Error.ToString(),
                    Type = "max_connections_limit",
                    Text = $"Процент подключений к бд занят на {processorPercentLoading} процентов",
                };

                problems.Add(problem);
            }

            var conProc = server.Settings.MaxLoadingProcessor * 0.2;
            var conDiff = stats[0].ConnectionInfo.ConnectionsUtilization - stats[^1].ConnectionInfo.ConnectionsUtilization;

            if (conDiff >= conProc)
            {
                var problem = new Problem
                {
                    Alert = AlertType.Error.ToString(),
                    Type = "max_connections_limit",
                    Text = $"Число подключений резко выросло на {(conDiff * 100) / server.Settings.MaxLoadingProcessor} процентов",
                };
            }

            return problems;
        }

        public async Task<int> GetProccessorPercentLoad(Server server)
        {
            if (!server.UseSSH)
            {
                return -1;
            }

            using (var client = new SshClient(server.HostnameSSH, server.PortSSH.Value, server.UsernameSSH, server.PasswordSSH))
            {
                client.Connect();
                string result;

                if (server.ServerOS == "Linux")
                {
                    result = client.RunCommand("vmstat 1 2 | awk 'END { print 100 - $15 }'").Result;
                }
                else // windows
                {
                    result = client.RunCommand("wmic cpu get loadpercentage").Result;
                    result = Regex.Match(result, @"\d+").Value;
                }
                
                client.Disconnect();
                return Convert.ToInt32(result);
            }
        }

        public async Task<bool> AddBackup(Server server, string filename)
        {
            if (!server.UseSSH)
            {
                return false;
            }

            using (var client = new SshClient(server.HostnameSSH, server.PortSSH.Value, server.UsernameSSH, server.PasswordSSH))
            {
                client.Connect();
                string result;

                if (server.ServerOS == "Linux")
                {
                    result = client.RunCommand($"sudo -u postgres pg_dump {server.DbName} > {filename + ".sql"}").Result;
                }
                else // windows
                {
                    //result = client.RunCommand("wmic cpu get loadpercentage").Result;
                    //result = Regex.Match(result, @"\d+").Value;
                }

                client.Disconnect();
                return true;
            }
        }

        public async Task<bool> UseBackup(Server server, string filename, string type)
        {
            if (!server.UseSSH)
            {
                return false;
            }

            using (var client = new SshClient(server.HostnameSSH, server.PortSSH.Value, server.UsernameSSH, server.PasswordSSH))
            {
                client.Connect();
                string result;

                if (server.ServerOS == "Linux")
                {
                    result = client.RunCommand($"sudo -u postgres pg_restore -U postgres -d {server.DbName} {type} {filename + ".sql"}").Result;
                }
                else // windows
                {
                    //result = client.RunCommand("wmic cpu get loadpercentage").Result;
                    //result = Regex.Match(result, @"\d+").Value;
                }

                client.Disconnect();
                return true;
            }
        }

        public async Task<bool> RestartDatabase(Server server)
        {
            if (!server.UseSSH)
            {
                return false;
            }

            using (var client = new SshClient(server.HostnameSSH, server.PortSSH.Value, server.UsernameSSH, server.PasswordSSH))
            {
                client.Connect();
                string result;

                if (server.ServerOS == "Linux")
                {
                    result = client.RunCommand($"sudo systemctl restart postgresql-13.service").Result;
                }
                else // windows
                {
                    // todo: write
                }

                client.Disconnect();
                return true;
            }
        }

        public async Task ClearIdleProcess(Server server)
        {
            var connectionString = $"Host={server.Host};Port={server.Port};Database={server.DbName};Username={server.Username};Password={server.Password}";

            var longConnections = await GetIdleConnections(connectionString);

            var halfHour = TimeSpan.FromMinutes(30);

            for (var i = 0; i < longConnections.Count; i++)
            {
                if (longConnections[i].Interval > halfHour)
                {
                    KillIdleProcess(server, longConnections[i].Pid);
                }
            }
        }

        // запрос на получение ингмофрмации о подключениях
        private async Task<ConnectionsInfo> GetConnections(string connectionString)
        {
            var query = "select A.total_connections, A.non_idle_connections,  B.max_connections,  round((100 * A.total_connections::numeric / B.max_connections::numeric), 2) connections_utilization_pctg from   (select count(1) as total_connections, sum(case when state!='idle' then 1 else 0 end) as non_idle_connections from pg_stat_activity) A,   (select setting as max_connections from pg_settings where name='max_connections') B;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int totalConnections = reader.GetInt32(0);
                            int nonIdleConnections = reader.GetInt32(1);
                            string maxConnections = reader.GetString(2);
                            double connectionsUtilization = reader.GetDouble(3);

                            return new ConnectionsInfo(totalConnections, nonIdleConnections, maxConnections, connectionsUtilization);
                        }
                    }
                }
            }

            return null;
        }

        // Запрос на получение размера базы данных в байтах
        private async Task<string> GetDatabaseSize(string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT pg_size_pretty(pg_database_size(datname)) AS database_size FROM pg_database WHERE datname = current_database()";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    // Выполнение запроса и получение результата
                    string size = (string)command.ExecuteScalar();

                    return size;
                }
            }
        }

        // Запрос на получение размера базы данных в байтах
        public async Task ReloadConfig(Server server)
        {
            var connectionString = $"Host={server.Host};Port={server.Port};Database={server.DbName};Username={server.Username};Password={server.Password}";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT pg_reload_conf();";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteScalar();
                }
            }
        }

        public void KillIdleProcess(Server server, int pid)
        {
            var connectionString = $"Host={server.Host};Port={server.Port};Database={server.DbName};Username={server.Username};Password={server.Password}";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var query = $@"SELECT pg_terminate_backend({pid});";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Запрос на получение долго простаивающих подключений
        private async Task<List<Connection>> GetIdleConnections(string connectionString)
        {
            var connections = new List<Connection>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var query = @"SELECT now() - query_start AS running_since, pid, datname, usename, state, left(query, 60) 
                      FROM pg_stat_activity 
                      WHERE state IN ('idle', 'idle in transaction') 
                      AND datname = current_database()
                      ORDER BY 1 DESC";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TimeSpan runningSince = reader.GetTimeSpan(0);
                            int pid = reader.GetInt32(1);
                            string datname = reader.GetString(2);
                            string usename = reader.GetString(3);
                            string state = reader.GetString(4);
                            string queryText = reader.GetString(5);

                            Connection connectionObj = new Connection
                            {
                                Interval = runningSince,
                                Pid = pid,
                                Datname = datname,
                                State = state,
                                Query = queryText
                            };

                            connections.Add(connectionObj);
                        }
                    }
                }

                return connections;
            }
        }
    }
}
