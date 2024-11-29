using System;

namespace PlayaApiV2.Model
{
    public class ApiException : Exception
    {
        public int Code { get; private set; }
        public int OriginalMessage { get; private set; }

        public ApiException() : base() { }

        public ApiException(
            int code = ApiStatusCodes.ERROR,
            string message = null,
            Exception innerException = null) : base(message, innerException)
        {
            Code = code;
        }

        public override string ToString() => $"({Code}) {Message}";
    }
}
