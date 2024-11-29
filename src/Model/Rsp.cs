using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public struct Rsp
    {
        [JsonProperty("status")]
        public ApiStatus Status { get; private set; }


        public Rsp(ApiStatus status) { Status = status; }

        public static implicit operator Rsp(ApiStatus status) => new Rsp(status);
    }

    public struct Rsp<T>
    {
        [JsonProperty("status")]
        public ApiStatus Status { get; private set; }

        [JsonProperty("data")]
        public T Data { get; private set; }


        public Rsp(ApiStatus status, T data) { Status = status; Data = data; }

        public static implicit operator Rsp<T>(ApiStatus status) => new Rsp<T>(status, default);
        public static implicit operator Rsp<T>(T data) => new Rsp<T>(ApiStatus.OK, data);
        public static implicit operator Rsp<T>(Rsp response) => new Rsp<T>(response.Status, default);
    }
}
