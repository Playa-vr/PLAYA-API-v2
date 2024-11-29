using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PlayaApiV2.Model
{
    [Serializable]
    public readonly struct SortDirection : IEquatable<SortDirection>
    {
        [JsonProperty]
        public string Value { get; }

        [JsonConstructor]
        public SortDirection(string value) => Value = value;

        [JsonIgnore] public bool HasValue => !string.IsNullOrEmpty(Value);

        public override bool Equals(object obj) => obj is SortDirection direction && Equals(direction);

        public bool Equals(SortDirection other) => Value == other.Value;

        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => Value;

        [Conditional("DEBUG")]
        public void EnsureSupported(IEnumerable<SortDirection> supportedValues)
        {
            if (Value != null && supportedValues != null && !supportedValues.Contains(this))
                throw new Exception($"Unsupported SortDirection {Value}. Expected: {string.Join(", ", supportedValues)}");
        }

        public static implicit operator SortDirection(string value) => new(value);

        public static bool operator ==(SortDirection left, SortDirection right) => left.Equals(right);

        public static bool operator !=(SortDirection left, SortDirection right) => !(left == right);
    }

    [Serializable]
    public readonly struct SortOrder : IEquatable<SortOrder>
    {
        [JsonProperty]
        public string Value { get; }

        [JsonConstructor]
        public SortOrder(string value) => Value = value;

        [JsonIgnore] public bool HasValue => !string.IsNullOrEmpty(Value);

        public override bool Equals(object obj) => obj is SortOrder order && Equals(order);

        public bool Equals(SortOrder other) => Value == other.Value;

        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => Value;

        [Conditional("DEBUG")]
        public void EnsureSupported(IEnumerable<SortOrder> supportedValues)
        {
            if (Value != null && supportedValues != null && !supportedValues.Contains(this))
                throw new Exception($"Unsupported SortOrder {Value}. Expected: {string.Join(", ", supportedValues)}");
        }

        public static implicit operator SortOrder(string value) => new(value);

        public static bool operator ==(SortOrder left, SortOrder right) => left.Equals(right);

        public static bool operator !=(SortOrder left, SortOrder right) => !(left == right);
    }

    [Serializable]
    public readonly struct ContentType : IEquatable<ContentType>
    {
        [JsonProperty]
        public string Value { get; }

        [JsonConstructor]
        public ContentType(string value) => Value = value;

        [JsonIgnore] public bool HasValue => !string.IsNullOrEmpty(Value);

        public override bool Equals(object obj) => obj is ContentType type && Equals(type);

        public bool Equals(ContentType other) => Value == other.Value;

        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => Value;

        [Conditional("DEBUG")]
        public void EnsureSupported(IReadOnlyCollection<ContentType> supportedValues)
        {
            if (Value != null && supportedValues != null && !supportedValues.Contains(this))
                throw new Exception($"Unsupported ContentType {Value}. Expected: {string.Join(", ", supportedValues)}");
        }

        public static implicit operator ContentType(string value) => new(value);

        public static bool operator ==(ContentType left, ContentType right) => left.Equals(right);

        public static bool operator !=(ContentType left, ContentType right) => !(left == right);
    }

    public static class SortDirections
    {
        public static readonly SortDirection Ascending = new("asc");
        public static readonly SortDirection Descending = new("desc");

        public static readonly IReadOnlyList<SortDirection> Values = new[] { Ascending, Descending };
    }

    public static class SortOrders
    {
        public static readonly SortOrder Title = new("title");
        public static readonly SortOrder ReleaseDate = new("release_date");
        public static readonly SortOrder Popularity = new("popularity");
        public static readonly SortOrder Duration = new("duration");

        public static readonly IReadOnlyList<SortOrder> Videos = new[] { Title, ReleaseDate, Popularity, };
        public static readonly IReadOnlyList<SortOrder> Actors = new[] { Title, Popularity, };
        public static readonly IReadOnlyList<SortOrder> Studios = new[] { Title, Popularity, };
    }

    public static class ContentTypes
    {
        public static readonly ContentType Trailer = new("trailer");
        public static readonly ContentType Full = new("full");

        public static readonly IReadOnlyList<ContentType> Values = new[] { Trailer, Full };
    }

    public static class UnavailableReasons
    {
        public const string Login = "login";
        public const string Premium = "premium";
    }

    public static class ReservedCategories
    {
        public static readonly string Straight = "Straight";
        public static readonly string Gay = "Gay";
        public static readonly string Trans = "Trans";
    }
}
