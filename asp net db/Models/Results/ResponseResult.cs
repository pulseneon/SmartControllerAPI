namespace asp_net_db.Models.Results
{
    public class ResponseResult<T> where T : class
    {
        public int Status { get; set; }
        public T? Response { get; set; }

        public ResponseResult(int status, T? response)
        {
            Status = status;
            Response = response;
        }
    }
}
