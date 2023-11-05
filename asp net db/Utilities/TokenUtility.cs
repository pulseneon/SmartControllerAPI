namespace asp_net_db.Utilities
{
    public static class TokenUtility
    {
        static private readonly string _token = "4XvL1RHinNFNM6jALE4hPLM-ncIWZxGfk";
        public static bool ValidateToken(string token)
        {
            if (token == null)
            {
                return false;
            }

            return _token.Equals(token);
        }
    }
}
