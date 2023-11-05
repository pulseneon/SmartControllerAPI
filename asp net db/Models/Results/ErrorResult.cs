namespace asp_net_db.Models.Results
{
    public class ErrorResult
    {
        public int StatusCode { get; set; }
        public string ErrorText { get; set; }

        public ErrorResult(int status, string error)
        {
            StatusCode = status;
            ErrorText = error;
        }
    }
}
