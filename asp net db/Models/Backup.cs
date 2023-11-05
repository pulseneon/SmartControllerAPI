namespace asp_net_db.Models
{
    public class Backup
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string Filename { get; set; }
    }
}
