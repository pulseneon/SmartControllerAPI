namespace asp_net_db.Models.Dto
{
    public class SettingsDto
    {
        public int? MaxLoadingProcessor { get; set; }
        public int? DatabaseSizePercent { get; set; }
        public int? MaxConnectionsPercent { get; set; }
    }
}
