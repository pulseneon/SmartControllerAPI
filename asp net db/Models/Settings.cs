using asp_net_db.Models.Dto;

namespace asp_net_db.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public int MaxLoadingProcessor { get; set; }
        public int DatabaseSizePercent { get; set; }
        public int MaxConnectionsPercent { get; set; }

        public Settings()
        {
            MaxLoadingProcessor = 85;
            DatabaseSizePercent = 90;
            MaxConnectionsPercent = 95;
        }

        public void Update(SettingsDto dto)
        {
            if (dto.MaxLoadingProcessor != null)
            {
                MaxLoadingProcessor = dto.MaxLoadingProcessor.Value;
            }

            if (dto.DatabaseSizePercent != null)
            {
                DatabaseSizePercent = dto.DatabaseSizePercent.Value;
            }

            if (dto.MaxConnectionsPercent != null)
            {
                MaxConnectionsPercent = dto.MaxConnectionsPercent.Value;
            }
        }
    }
}
