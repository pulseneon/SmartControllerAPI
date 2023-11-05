namespace asp_net_db.Models
{
    public class Problem
    {
        public enum AlertType
        {
            Fatal,
            Error,
            Warning,
        }

        public string Alert { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }

        public Problem(string alert, string text)
        {
            Alert = alert;
            Text = text;
        }

        public Problem()
        {

        }
    }
}
