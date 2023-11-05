namespace asp_net_db.Utilities
{
    public class ConverterUtility
    {
        public static int GetMegabytes(string arg)
        {
            var split = arg.Split(' ');
            int.TryParse(split[0], out int size);

            switch (split[1])
            {
                case "kB":
                    size /= 1024;
                    break;
                case "GB":
                    size *= 1024;
                    break;
            }

            return size;
        }
    }
}
