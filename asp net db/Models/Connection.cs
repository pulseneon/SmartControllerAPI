namespace asp_net_db.Models
{
    public class Connection
    {
        public int ID { get; set; }
        public TimeSpan Interval { get; set; }
        public int Pid { get; set; }
        public string Datname { get; set; }
        public string State { get; set; }
        public string Query { get; set; }
    }
}
