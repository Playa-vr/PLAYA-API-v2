using Newtonsoft.Json;

using PlayaApiV2.Extensions;
using PlayaApiV2.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayaApiV2.Repositories
{
    // Todo: implement actors and studios
    public class VideosRepository
    {
        private readonly Dictionary<string, VideoView> _videos;

        public VideosRepository()
        {
            _videos = new Dictionary<string, VideoView>()
            {
                ["1"] = new VideoView
                {
                    Id = "1",
                    Title = "Video Title",
                    Subtitle = "Video Subtitle",
                    Description = "Video Description",
                    Preview = "https://www.sample-videos.com/img/Sample-jpg-image-100kb.jpg",
                    ReleaseDate = Timestamp.From(new DateTime(2024, 11, 21)),
                    Categories = new List<VideoView.CategoryRef>
                      {
                          new VideoView.CategoryRef
                          {
                              Id = "category_id",
                              Title = "Category Title",
                          },
                      },
                    Details = new List<VideoView.VideoDetails>
                      {
                          new VideoView.VideoDetails
                          {
                              Type = "trailer",
                              DurationSeconds = 5,
                              Links = new List<VideoLinkView>
                              {
                                  new VideoLinkView
                                  {
                                      IsStream = true,
                                      Url = "https://www.sample-videos.com/video321/mp4/720/big_buck_bunny_720p_1mb.mp4",
                                      QualityOrder = 15,
                                      QualityName = "720p",
                                      ProjectionString = "FLT",
                                      StereoString = "MN",
                                  },
                              },
                          },
                          new VideoView.VideoDetails
                          {
                              Type = "full",
                              DurationSeconds = 62,
                              Links = new List<VideoLinkView>
                              {
                                  new VideoLinkView
                                  {
                                      IsStream = true,
                                      IsDownload = true,
                                      Url = "https://www.sample-videos.com/video321/mp4/720/big_buck_bunny_720p_10mb.mp4",
                                      QualityOrder = 15,
                                      QualityName = "720p",
                                      ProjectionString = "FLT",
                                      StereoString = "MN",
                                  },
                                  new VideoLinkView
                                  {
                                      IsStream = true,
                                      IsDownload = true,
                                      Url = null,
                                      UnavailableReason = "Premium",
                                      QualityOrder = 25,
                                      QualityName = "1080p",
                                      ProjectionString = "FLT",
                                      StereoString = "MN",
                                  }
                              }
                          },
                      }
                },
            };
        }

        public async Task<Page<VideoListView>> GetVideosAsync(VideosQuery query)
        {
            // Todo: implement pagination, searching by actors and studios

            IEnumerable<VideoView> videos = _videos.Values;

            if (!string.IsNullOrEmpty(query.SearchTitle))
                videos = videos.Where(v => v.Title.Contains(query.SearchTitle, StringComparison.OrdinalIgnoreCase));
            if (!query.ExcludedCategories.IsNullOrEmpty())
                videos = videos.Where(e => !query.ExcludedCategories.Overlaps(e.Categories.OrEmpty().Select(c => c.Id)));
            if (!query.IncludedCategories.IsNullOrEmpty())
                videos = videos.Where(e => query.IncludedCategories.IsSubsetOf(e.Categories.OrEmpty().Select(c => c.Id)));
            if (query.SortOrder == SortOrders.Title)
                videos = OrderBy(videos, e => e.Title, query.SortDirection, s_naturalComparer);
            if (query.SortOrder == SortOrders.ReleaseDate)
                videos = OrderBy(videos, e => e.ReleaseDate.GetValueOrDefault(), query.SortDirection);
            if (query.SortOrder == SortOrders.Popularity)
                videos = OrderBy(videos, e => e.Views, query.SortDirection);
            if (query.SortOrder == SortOrders.Duration)
                videos = OrderBy(videos, e => e.Details?.FirstOrDefault()?.DurationSeconds.GetValueOrDefault() ?? 0, query.SortDirection);

            var videoViews = videos.Select(e => new VideoListView
            {
                Id = e.Id,
                Title = e.Title,
                Subtitle = e.Subtitle,
                Preview = e.Preview,
                ReleaseDate = e.ReleaseDate,
                Details = e.Details.Select(d => new VideoListView.VideoDetails
                {
                    Type = d.Type,
                    DurationSeconds = d.DurationSeconds,
                }).ToList(),
            }).ToList();

            return new Page<VideoListView>(videoViews);
        }

        public async Task<VideoView> GetVideoAsync(string videoId)
        {
            if (_videos.TryGetValue(videoId, out var video))
                return video;
            throw GetNotFoundException();
            Exception GetNotFoundException() => new ApiException(ApiStatusCodes.NOT_FOUND, $"Video '{videoId}' not found");
        }

        public async Task<List<CategoryListView>> GetCategoriesAsync(bool onlyPublished = true)
        {
            IEnumerable<VideoView> videos = _videos.Values;

            return videos
                .SelectMany(v => v.Categories)
                .Distinct()
                .OrderBy(c => c.Title, NaturalComparer.Default)
                .Select(c => new CategoryListView { Id = c.Id, Title = c.Title, })
                .ToList();
        }

        private static readonly NaturalComparer s_naturalComparer = new NaturalComparer(NaturalComparer.NaturalComparerOptions.Default, StringComparison.OrdinalIgnoreCase);
        private static IEnumerable<T> OrderBy<T, K>(IEnumerable<T> collection, Func<T, K> selector, SortDirection sortDirection, IComparer<K> comparer = null)
        {
            if (sortDirection == SortDirections.Ascending)
                return collection.OrderBy(selector, comparer);
            else if (sortDirection == SortDirections.Descending)
                return collection.OrderByDescending(selector, comparer);
            else
                return collection;
        }
    }

    public class VideosQuery : IEquatable<VideosQuery>
    {
        public long PageIndex { get; set; }
        public long PageSize { get; set; }

        public SortOrder SortOrder { get; set; }
        public SortDirection SortDirection { get; set; }

        public string SearchTitle { get; set; }
        public string StudioId { get; set; }
        public string ActorId { get; set; }

        /// <summary>
        /// All
        /// </summary>
        public HashSet<string> IncludedCategories { get; } = new HashSet<string>();

        /// <summary>
        /// None
        /// </summary>
        public HashSet<string> ExcludedCategories { get; } = new HashSet<string>();

        public void AddIncludedCategories(IEnumerable<string> categories) => AddRange(categories, IncludedCategories);
        public void AddExcludedCategories(IEnumerable<string> categories) => AddRange(categories, ExcludedCategories);

        private void AddRange<T>(IEnumerable<T> from, HashSet<T> to)
        {
            if (from == null)
                return;

            foreach (var category in from)
                to.Add(category);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VideosQuery);
        }

        public bool Equals(VideosQuery other)
        {
            return other is not null &&
                   PageIndex == other.PageIndex &&
                   PageSize == other.PageSize &&
                   SortOrder.Equals(other.SortOrder) &&
                   SortDirection.Equals(other.SortDirection) &&
                   SearchTitle == other.SearchTitle &&
                   StudioId == other.StudioId &&
                   ActorId == other.ActorId &&
                   IncludedCategories.SetEquals(other.IncludedCategories) &&
                   ExcludedCategories.SetEquals(other.ExcludedCategories);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(PageIndex);
            hash.Add(PageSize);
            hash.Add(SortOrder);
            hash.Add(SortDirection);
            hash.Add(SearchTitle);
            hash.Add(StudioId);
            hash.Add(ActorId);
            //hash.Add(IncludedCategories);
            //hash.Add(ExcludedCategories);
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static bool operator ==(VideosQuery left, VideosQuery right)
        {
            return EqualityComparer<VideosQuery>.Default.Equals(left, right);
        }

        public static bool operator !=(VideosQuery left, VideosQuery right)
        {
            return !(left == right);
        }
    }
}
