using Newtonsoft.Json;

using Semver;

using System;

namespace PlayaApiV2.Model
{
    public class SemVersionConverter : JsonConverter<SemVersion>
    {
        public const SemVersionStyles Styles = SemVersionStyles.OptionalMinorPatch;
        public static readonly SemVersionConverter Default = new SemVersionConverter();
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { Converters = new[] { Default } };

        public override SemVersion ReadJson(JsonReader reader, Type objectType, SemVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return FromString(reader.Value as string);
        }

        public override void WriteJson(JsonWriter writer, SemVersion value, JsonSerializer serializer)
        {
            writer.WriteValue(ToString(value));
        }

        public static bool TryFromString(string value, out SemVersion version)
        {
            return SemVersion.TryParse(value, Styles, out version);
        }

        public static SemVersion FromString(string value)
        {
            return string.IsNullOrEmpty(value) ? null : SemVersion.Parse(value, Styles);
        }

        public static string ToString(SemVersion value)
        {
            return value?.ToString();
        }
    }

    public static class SemVersionExtensions
    {
        public static bool Less(this SemVersion l, SemVersion r)
        {
            return l.ComparePrecedenceTo(r) < 0;
        }

        public static bool LessOrEquals(this SemVersion l, SemVersion r)
        {
            return l.ComparePrecedenceTo(r) <= 0;
        }

        public static bool Greater(this SemVersion l, SemVersion r)
        {
            return l.ComparePrecedenceTo(r) > 0;
        }

        public static bool GreaterOrEquals(this SemVersion l, SemVersion r)
        {
            return l.ComparePrecedenceTo(r) >= 0;
        }

        public static bool Equals(this SemVersion l, SemVersion r)
        {
            return l.ComparePrecedenceTo(r) == 0;
        }

        public static bool NotEquals(this SemVersion l, SemVersion r)
        {
            return l.ComparePrecedenceTo(r) != 0;
        }
    }
}
