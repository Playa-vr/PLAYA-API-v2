namespace PlayaApiV2.Model
{
    /// <summary>
    /// Use with <see cref="Rsp"/>
    /// </summary>
    public static class ApiStatusCodes
    {
        public const int GENERAL_FAILURE = 0;
        public const int OK = 1;
        public const int ERROR = 2;
        public const int AUTHORIZATION_FAILED = 3;

        public const int UNAUTHORIZED = 401;
        public const int FORBIDDEN = 403;
        public const int NOT_FOUND = 404;
        public const int UNDER_MAINTENANCE = 503;
    }
}
