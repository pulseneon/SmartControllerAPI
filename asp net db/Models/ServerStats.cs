namespace asp_net_db.Models
{
    public class ServerStats
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public double ProcessorPercentLoading { get; set; }
        public string DatabaseSize { get; set; }
        public List<Connection> LongIdleConnections { get; set; }
        public ConnectionsInfo ConnectionInfo { get; set; }    
        //public List<Process> LongIdleProcesses { get; set; }
        public long WritedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
