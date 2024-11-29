using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public struct ApiStatus
    {
        [JsonProperty("code")]
        public int Code { get; private set; }

        [JsonProperty("message")]
        public string Message { get; private set; }

        public ApiStatus(int code, string message)
        {
            Code = code;
            Message = message;
        }

        [JsonIgnore]
        public bool IsSuccessful => Code == ApiStatusCodes.OK;
        public void EnsureSuccessful()
        {
            if (!IsSuccessful)
                throw new ApiException(Code, Message);
        }

        public static readonly ApiStatus OK = new ApiStatus(ApiStatusCodes.OK, string.Empty);
        public static ApiStatus From(int code) => new ApiStatus(code, string.Empty);
        public static ApiStatus From(string message) => new ApiStatus(ApiStatusCodes.ERROR, message);
        public static ApiStatus From(int code, string message) => new ApiStatus(code, message);

        public override string ToString()
        {
            return $"Code: {Code} Message: {Message}";
        }
    }
}
