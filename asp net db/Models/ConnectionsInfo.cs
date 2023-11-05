namespace asp_net_db.Models
{
    public class ConnectionsInfo
    {
        public int Id { get; set; }
        public int TotalConnections { get; set; }
        public int NonIdleConnections { get; set; }
        public string MaxConnections { get; set; }
        public double ConnectionsUtilization { get; set; }

        public ConnectionsInfo(int totalConnections, int nonIdleConnections, string maxConnections, double connectionsUtilization)
        {
            TotalConnections = totalConnections;
            NonIdleConnections = nonIdleConnections;
            MaxConnections = maxConnections;
            ConnectionsUtilization = connectionsUtilization;
        }
    }
}
