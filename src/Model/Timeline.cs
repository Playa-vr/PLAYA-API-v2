using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace PlayaApiV2.Model
{
    [Serializable]
    [JsonConverter(typeof(TimelineConverter))]
    public class Timeline
    {
        #region Static
        public static Timeline Example => _example ??= FromCompressed(new Marker[]
        {
            new(0, title: "Start"),
            new(5000, zoom: 1),
            new(10000, zoom: 0, height: -1),
            new(15000, tilt: 45),
            new(20000, height: 1),
            new(25000, tilt: 0),
            new(30000, title: "Middle", zoom: -1, tilt: -30),
            new(35000, title: "End"),
            new(40000, zoom: 0, tilt: 0, height: 0),
        });
        private static Timeline _example;

        public static Timeline Empty => _empty ??= FromCompressed(Array.Empty<Marker>());
        private static Timeline _empty;

        /// <summary>
        /// From compressed data
        /// </summary>
        public static Timeline FromCompressed(IReadOnlyList<Marker> readOnlyCompressedMarkers)
        {
            if (readOnlyCompressedMarkers == null)
                return null;

            var compressedMarkers = readOnlyCompressedMarkers.ToList();
            var decompressedMarkers = new List<Marker>();

            if (compressedMarkers.Count > 0)
            {
                compressedMarkers.Sort(MarkerComparer.Default);

                static Marker Decompress(Marker currentCompressed, Marker? prevDecompressed, List<Marker> decompressedMarkers)
                {
                    if (prevDecompressed.HasValue)
                    {
                        var value = prevDecompressed.Value;
                        var currentDecompressed = Marker.Decompress(currentCompressed, value);
                        if (currentDecompressed.HasSpecificValues() || currentDecompressed.HasDifferences(value))
                        {
                            if (currentDecompressed.TimeMs == value.TimeMs)
                                decompressedMarkers[decompressedMarkers.Count - 1] = currentDecompressed;
                            else
                                decompressedMarkers.Add(currentDecompressed);
                        }
                        return currentDecompressed;
                    }
                    else
                    {
                        decompressedMarkers.Add(currentCompressed);
                        return currentCompressed;
                    }
                }

                var initialMarker = new Marker();
                decompressedMarkers.Add(initialMarker);

                var prevDecompressed = Decompress(compressedMarkers[0], initialMarker, decompressedMarkers);
                for (int i = 1; i < compressedMarkers.Count; i++)
                    prevDecompressed = Decompress(compressedMarkers[i], prevDecompressed, decompressedMarkers);
            }

            return new Timeline(decompressedMarkers);
        }

        /// <summary>
        /// To compressed data
        /// </summary>
        public static List<Marker> ToCompressed(Timeline timeline)
        {
            if (timeline == null)
                return null;

            var decompressedMarkers = timeline._decompressedMarkers;
            var compressedMarkers = new List<Marker>();

            if (decompressedMarkers.Count > 0)
            {
                static Marker Compress(Marker currentDecompressed, Marker? prevDecompressed, List<Marker> compressedMarkers)
                {
                    if (prevDecompressed.HasValue)
                    {
                        var value = prevDecompressed.Value;
                        var compressed = Marker.Compress(currentDecompressed, value);
                        if (compressed.HasValues())
                            compressedMarkers.Add(compressed);
                        return compressed;
                    }
                    else
                    {
                        compressedMarkers.Add(currentDecompressed);
                        return currentDecompressed;
                    }
                }

                var prevDecompressed = decompressedMarkers[0];
                Compress(prevDecompressed, default, compressedMarkers);
                for (int i = 1; i < decompressedMarkers.Count; i++)
                {
                    var currentDecompressed = decompressedMarkers[i];
                    Compress(currentDecompressed, prevDecompressed, compressedMarkers);
                    prevDecompressed = currentDecompressed;
                }
            }

            return compressedMarkers;
        }
        #endregion

        private List<Marker> _decompressedMarkers;
        private List<Marker> _titleMarkers;

        private Timeline(List<Marker> decompressedMarkers)
        {
            _decompressedMarkers = decompressedMarkers;
        }

        public int Count => _decompressedMarkers.Count;
        public Marker this[int index] => _decompressedMarkers[index];

        private int BinarySearch(int timeMs)
        {
            var index = _decompressedMarkers.BinarySearch(new Marker { TimeMs = timeMs }, MarkerComparer.Default);
            if (index < 0)
                index = ~index - 1;
            return index;
        }

        public bool TryGetMarkerIndex(int timeMs, out int index)
        {
            index = BinarySearch(timeMs);
            return index >= 0 && index < _decompressedMarkers.Count;
        }



        public IReadOnlyList<Marker> GetTitleMarkers()
        {
            return _titleMarkers ??= _decompressedMarkers.Where(m => m.HasTitle).ToList();
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct Marker
    {
        /// <summary>
        /// Time in miliseconds from start of video
        /// </summary>
        [JsonProperty("time")]
        public int TimeMs { get; set; }

        //Optional values.
        //If null - use value from previous Marker where this value exists

        /// <summary>
        /// [-1 1]
        /// </summary>
        [JsonProperty("zoom")]
        public float? Zoom { get; set; }

        /// <summary>
        /// [-1 1]
        /// </summary>
        [JsonProperty("height")]
        public float? Height { get; set; }

        /// <summary>
        /// Camera rotation UP or DOWN [-90 90]. Positive values is UP
        /// </summary>
        [JsonProperty("tilt")]
        public float? Tilt { get; set; }

        /// <summary>
        /// Name of video segment (as in youtube).
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// Means this property is valid keyframe and not propagated from previous marker value
        /// </summary>
        [JsonIgnore]
        public bool HasTitle { get; set; }


        public Marker(int timeMs, float? zoom = null, float? height = null, float? tilt = null, string title = null)
        {
            TimeMs = timeMs;
            Zoom = zoom;
            Height = height;
            Tilt = tilt;
            Title = title;
            HasTitle = !string.IsNullOrEmpty(title);
        }

        public bool HasValues() =>
            false
            || Zoom.HasValue
            || Height.HasValue
            || Tilt.HasValue
            || HasTitle
            ;

        /// <summary>
        /// Compare everything except <see cref="TimeMs"/>
        /// </summary>
        public bool HasDifferences(Marker marker) =>
            false
            || Zoom != marker.Zoom
            || Height != marker.Height
            || Tilt != marker.Tilt
            || Title != marker.Title
            ;

        public bool HasSpecificValues() =>
            false
            || HasTitle
            ;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            HasTitle = !string.IsNullOrEmpty(Title);
        }

        private string DebuggerDisplay
        {
            get
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        /// <summary>
        /// Replcae null values with values from <paramref name="prev"/> 
        /// </summary>
        /// <param name="prev"></param>
        public static Marker Decompress(Marker current, Marker prev)
        {
            return new Marker
            {
                TimeMs = current.TimeMs,
                Zoom = current.Zoom ?? prev.Zoom,
                Height = current.Height ?? prev.Height,
                Tilt = current.Tilt ?? prev.Tilt,

                Title = current.Title ?? prev.Title,
                HasTitle = current.HasTitle,
            };
        }

        /// <summary>
        /// Set values that equal to null
        /// </summary>
        /// <param name="prev"></param>
        public static Marker Compress(Marker current, Marker prev)
        {
            return new Marker
            {
                TimeMs = current.TimeMs,
                Zoom = NullIfEquals(current.Zoom, prev.Zoom),
                Height = NullIfEquals(current.Height, prev.Height),
                Tilt = NullIfEquals(current.Tilt, prev.Tilt),

                Title = NullIfFalse(current.Title, current.HasTitle),
                HasTitle = current.HasTitle,
            };

            static T? NullIfEquals<T>(T? current, T? prev) where T : struct, IEquatable<T> => EqualityComparer<T?>.Default.Equals(current, prev) ? null : current;
            static T NullIfFalse<T>(T current, bool condition) where T : class => condition ? current : null;
        }
    }

    public class MarkerComparer : IComparer<Marker>
    {
        public static readonly MarkerComparer Default = new MarkerComparer(false);
        public static readonly MarkerComparer Reversed = new MarkerComparer(true);

        public readonly bool IsReversed;

        public MarkerComparer(bool reversed)
        {
            IsReversed = reversed;
        }

        public int Compare(Marker x, Marker y)
        {
            var result = x.TimeMs.CompareTo(y.TimeMs);
            return IsReversed ? -result : result;
        }
    }

    public class TimelineConverter : JsonConverter<Timeline>
    {
        public override Timeline ReadJson(JsonReader reader, Type objectType, Timeline existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var markers = serializer.Deserialize<List<Marker>>(reader);
            return Timeline.FromCompressed(markers);
        }

        public override void WriteJson(JsonWriter writer, Timeline value, JsonSerializer serializer)
        {
            var markers = Timeline.ToCompressed(value);
            serializer.Serialize(writer, markers);
        }
    }
}