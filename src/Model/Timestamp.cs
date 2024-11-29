using Newtonsoft.Json;

using System;
using System.Globalization;

namespace PlayaApiV2.Model
{
    /// <summary>
    /// Time in unix seconds
    /// </summary>
    [JsonConverter(typeof(TimestampConverter))]
    public struct Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>
    {
        public long Ts { get; set; }
        public DateTime Dt { get => Ts.FromUnixSeconds().UtcDateTime; set => Ts = value.ToUnixSeconds(); }
        public DateTimeOffset Dto { get => Ts.FromUnixSeconds(); set => Ts = value.ToUnixSeconds(); }

        public Timestamp(long ts)
        {
            Ts = ts;
        }

        /// <summary>
        /// Loses precission
        /// </summary>
        public Timestamp(DateTime dt)
        {
            Ts = dt.ToUnixSeconds();
        }

        /// <summary>
        /// Loses precission
        /// </summary>
        public Timestamp(DateTimeOffset dt)
        {
            Ts = dt.ToUnixSeconds();
        }

        public int CompareTo(Timestamp other)
        {
            return Ts.CompareTo(other.Ts);
        }

        public override bool Equals(object obj)
        {
            return obj is Timestamp timestamp && Equals(timestamp);
        }

        public bool Equals(Timestamp other)
        {
            return Ts == other.Ts;
        }

        public override int GetHashCode()
        {
            return Ts.GetHashCode();
        }

        public static bool operator ==(Timestamp left, Timestamp right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Timestamp left, Timestamp right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return Ts.ToString(CultureInfo.InvariantCulture);
        }

        public static Timestamp From(DateTime dateTime)
        {
            return new Timestamp(dateTime);
        }
        public static Timestamp From(long dateTime)
        {
            return new Timestamp(dateTime);
        }

        public static Timestamp? From(DateTime? dateTime)
        {
            return dateTime.HasValue ? new Timestamp(dateTime.Value) : default(Timestamp?);
        }
        public static Timestamp? From(long? dateTime)
        {
            return dateTime.HasValue ? new Timestamp(dateTime.Value) : default(Timestamp?);
        }
    }

    public class TimestampConverter : JsonConverter<Timestamp>
    {
        public override Timestamp ReadJson(JsonReader reader, Type objectType, Timestamp existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                throw new JsonException($"Invalid timestamp format at json path {reader.Path}");
            var str = reader.Value.ToString();
            var l = long.Parse(str, CultureInfo.InvariantCulture);
            return new Timestamp(l);
        }

        public override void WriteJson(JsonWriter writer, Timestamp value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Ts);
        }
    }

    public static class TimestampExtensions
    {
        /// <summary>
        /// To unix seconds. Loses precission
        /// </summary>
        public static long ToUnixSeconds(this DateTime dateTime) => ((DateTimeOffset)dateTime).ToUnixSeconds();

        /// <summary>
        /// To unix seconds. Loses precission
        /// </summary>
        public static long ToUnixSeconds(this DateTimeOffset dateTime)
        {
            var timestamp = dateTime.ToUniversalTime().ToUnixTimeSeconds();
            return timestamp;
        }

        /// <summary>
        /// From unix seconds
        /// </summary>
        public static DateTimeOffset FromUnixSeconds(this long timestamp)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return dateTimeOffset;
        }
    }
}
