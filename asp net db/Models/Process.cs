using NuGet.Packaging.Signing;

namespace asp_net_db.Models
{
    public class Process
    {
        public int datid { get; set; }
        public string datname { get; set; }
        public int pid { get; set; }
        public string username { get; set; }
        public long backend_start { get; set; }
        public long query_start { get; set; }
        public long state_change { get; set; }
        public string state { get; set; }
        public string query { get; set; }

    }
}
