using asp_net_db.Data;
using asp_net_db.Migrations;
using asp_net_db.Models;
using asp_net_db.Models.Dto;
using asp_net_db.Models.Results;
using asp_net_db.Services;
using asp_net_db.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace asp_net_db.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly ServerService _serverService;

        public ServerController(ApplicationContext context, IMapper mapper, ServerService serverService)
        {
            _context = context;
            _mapper = mapper;
            _serverService = serverService;
        }

        /// <summary>
        /// Добавить новый сервер
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddServer([FromBody] ServerDto model, string token)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = _mapper.Map<Server>(model);

            await _context.Servers.AddAsync(server);

            await _context.SaveChangesAsync();

            return StatusCode(200, new ResponseResult<Server>(200, server));
        }

        /// <summary>
        /// Получить список серверов
        /// </summary>
        [HttpGet("servers")]
        public async Task<IActionResult> GetServersList(string token)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var servers = await _context.Servers.AsNoTracking().ToListAsync();

            if (servers.Count == 0)
            {
                return StatusCode(200, new ResponseResult<string>(200, "Нет доступных серверов"));
            }

            return StatusCode(200, new ResponseResult<List<Server>>(200, servers));
        }

        /// <summary>
        /// Информация о сервере
        /// </summary>
        [HttpGet("server")]
        public async Task<IActionResult> GetServer(string token, int id)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == id);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            return StatusCode(200, new ResponseResult<Server>(200, server));
        }

        /// <summary>
        /// Настройки сервера
        /// </summary>
        [HttpGet("settings")]
        public async Task<IActionResult> EditSettings(string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            return StatusCode(200, new ResponseResult<Settings>(200, server.Settings));
        }

        /// <summary>
        /// Изменить настройки сервера
        /// </summary>
        [HttpPost("updatesettings")]
        public async Task<IActionResult> EditSettings([FromBody] SettingsDto settingsDto, string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            var settings = await _context.Settings.FirstOrDefaultAsync(x => x.Id == server.Settings.Id);

            //if (settingsDto != null)
            //{
            //    if (settingsDto.MaxLoadingProcessor != null)
            //    {
            //        settings.MaxLoadingProcessor = settingsDto.MaxLoadingProcessor.Value;
            //    }

            //    if (settingsDto.MaxConnectionsPercent != null)
            //    {
            //        settings.MaxConnectionsPercent = settingsDto.MaxConnectionsPercent.Value;
            //    }

            //    if (settingsDto.DatabaseSizePercent != null)
            //    {
            //        settings.DatabaseSizePercent = settingsDto.DatabaseSizePercent.Value;
            //    }
            //}


            // server.Settings.Update(settings);
            //            _context.Settings.Update(server.Settings);

            await _context.SaveChangesAsync();

            return StatusCode(200, new ResponseResult<Settings>(200, server.Settings));
        }

        /// <summary>
        /// Информация состоянии о сервера
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetServerStats(string token, int id)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == id);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            var stats = await _serverService.GetServerMetrics(server);

            return StatusCode(200, new ResponseResult<ServerStats>(200, stats));
        }

        /// <summary>
        /// Информация процессе на сервере
        /// </summary>
        [HttpGet("getprocess")]
        public async Task<IActionResult> GetProcess(string token, int id)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            return Ok();
        }

        /// <summary>
        /// Убить все долгие процессы в ожидании
        /// </summary>
        [HttpPost("clearprocess")]
        public async Task<IActionResult> KillProcess(string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            await _serverService.ClearIdleProcess(server);

            return Ok();
        }

        /// <summary>
        /// Убить процесс
        /// </summary>
        [HttpPost("killprocess")]
        public async Task<IActionResult> KillProcess(string token, int serverId, int pid)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            _serverService.KillIdleProcess(server, pid);

            return Ok();
        }

        /// <summary>
        /// Найти проблемы сервера
        /// </summary>
        [HttpGet("findproblems")]
        public async Task<IActionResult> FindProblems(string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var stats = await _serverService.GetServerMetrics(server);
            await _context.ServerStats.AddAsync(stats);
            await _context.SaveChangesAsync();

            var problems = await _serverService.FindProblems(server);

            return StatusCode(200, new ResponseResult<List<Problem>>(200, problems));
        }

        /// <summary>
        /// Последние записи
        /// </summary>
        [HttpGet("lastrecords")]
        public async Task<IActionResult> LastStatsRecords(string token, int serverId, int records = 10)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            var stats = await _context.ServerStats.OrderByDescending(x => x.WritedAt).Include(x => x.ConnectionInfo).Include(x => x.LongIdleConnections).Where(x => x.ServerId == serverId).Take(records).ToListAsync();

            return StatusCode(200, new ResponseResult<List<ServerStats>>(200, stats));
        }

        /// <summary>
        /// Перезагрузка конфига сервера
        /// </summary>
        [HttpGet("reloadconf")]
        public async Task<IActionResult> ReloadConfig(string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingServer(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            await _serverService.ReloadConfig(server);

            return Ok();
        }

        /// <summary>
        /// Пинг ssh сервера
        /// </summary>
        [HttpGet("pingssh")]
        public async Task<IActionResult> PingSSH(string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var pingResult = await _serverService.PingSSH(server);
            if (pingResult != null) return StatusCode(503, pingResult);

            return Ok();
        }


        /// <summary>
        /// Бекапы сервера
        /// </summary>
        [HttpGet("getbackups")]
        public async Task<IActionResult> GetBackupsList(string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var backups = await _context.Backups.Where(x => x.ServerId == serverId).ToListAsync();

            if (backups.Count == 0)
            {
                return StatusCode(200, new ResponseResult<string>(200, "Нет доступных бекапов"));
            }

            return StatusCode(200, new ResponseResult<List<Backup>>(200, backups));
        }

        /// <summary>
        /// Начать бекап сервера
        /// </summary>
        [HttpPost("createbackup")]
        public async Task<IActionResult> CreateBackup(string token, int serverId, string filename)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var backupResult = await _serverService.AddBackup(server, filename); 

            await _context.Backups.AddAsync(new Backup()
            {
                Filename = filename,
                ServerId = serverId,
            });

            await _context.SaveChangesAsync();

            return (backupResult) ? StatusCode(200, new ResponseResult<string>(200, "Бекап успешно создан")) :
                    StatusCode(503, new ErrorResult(503, "Бекап не был создан по каким-то причинам"));
        }

        /// <summary>
        /// Начать бекап сервера
        /// </summary>
        [HttpGet("usebackup")]
        public async Task<IActionResult> UseBackup(string token, int serverId, int backupId, string backuptype)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            if (server == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Сервера с таким ID не найдено"));
            }

            var backup = await _context.Backups.FirstOrDefaultAsync(x => x.Id == backupId);

            if (backup == null)
            {
                return StatusCode(404, new ErrorResult(404, error: "Бекапа с таким ID не найдено"));
            }

            if (backuptype != "-a" && backuptype != "-c" && backuptype != "-C")
            {
                return StatusCode(404, new ErrorResult(404, error: "Такого типа бекапа нет (-a: восстановить только данные.\r\n-c: удалить объекты базы данных перед их воссозданием.\r\n-C: создает базу данных перед восстановлением в нее.)"));
            }

            await _serverService.UseBackup(server, backup.Filename, backuptype);

            return Ok();
        }

        /// <summary>
        /// Начать бекап сервера
        /// </summary>
        [HttpGet("restart")]
        public async Task<IActionResult> RestartDatabase(string token, int serverId)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401, new ErrorResult(401, error: "Невалидный токен"));

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id == serverId);

            _serverService.RestartDatabase(server);

            return Ok();
        }
    }
}
