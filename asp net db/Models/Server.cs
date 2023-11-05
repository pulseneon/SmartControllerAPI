namespace asp_net_db.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string DisplayedName { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Port { get; set; }
        public string DbName { get; set; }
        public string Password { get; set; }
        public int AllocatedSpace { get; set; }

        public bool UseSSH { get; set; }
        public string? ServerOS { get; set; }
        public int? PortSSH { get; set; }
        public string? HostnameSSH { get; set; }
        public string? UsernameSSH { get; set; }
        public string? PasswordSSH { get; set; }

        public bool IsDeleted { get; set; } = false;
        public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public Settings Settings { get; set; } = new Settings();
        
        public Server()
        {

        }
    }
}
