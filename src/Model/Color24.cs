using Newtonsoft.Json;

using System;
using System.Globalization;

namespace PlayaApiV2.Model
{
    [Serializable]
    public struct Color24 : IFormattable, IEquatable<Color24>
    {
        [JsonProperty]
        public byte r;

        [JsonProperty]
        public byte g;

        [JsonProperty]
        public byte b;

        [JsonIgnore]
        public byte this[int index]
        {
            get
            {
                return index switch
                {
                    0 => r,
                    1 => g,
                    2 => b,
                    _ => throw new IndexOutOfRangeException("Invalid Color24 index(" + index + ")!"),
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        r = value;
                        break;
                    case 1:
                        g = value;
                        break;
                    case 2:
                        b = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Color24 index(" + index + ")!");
                }
            }
        }

        public Color24(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }


        public static bool operator ==(Color24 left, Color24 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color24 left, Color24 right)
        {
            return !(left == right);
        }

        public static Color24 Lerp(Color24 a, Color24 b, float t)
        {
            t = Clamp01(t);
            return new Color24((byte)(a.r + (b.r - a.r) * t), (byte)(a.g + (b.g - a.g) * t), (byte)(a.b + (b.b - a.b) * t));
        }

        public static Color24 LerpUnclamped(Color24 a, Color24 b, float t)
        {
            return new Color24((byte)(a.r + (b.r - a.r) * t), (byte)(a.g + (b.g - a.g) * t), (byte)(a.b + (b.b - a.b) * t));
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return string.Format("RGBA({0}, {1}, {2})", r.ToString(format, formatProvider), g.ToString(format, formatProvider), b.ToString(format, formatProvider));
        }

        public override bool Equals(object obj)
        {
            return obj is Color24 color && Equals(color);
        }

        public bool Equals(Color24 other)
        {
            return r == other.r &&
                   g == other.g &&
                   b == other.b;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(r, g, b);
        }

        private static float Clamp01(float value)
        {
            return MathF.Max(0, Math.Min(value, 1));
        }
    }
}
