using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using PlayaApiV2.Filters;
using PlayaApiV2.Model;
using PlayaApiV2.Model.Events;
using PlayaApiV2.Repositories;

using Semver;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayaApiV2.Controllers
{
    [Route("api/playa/v2")]
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    public class VideosController : ControllerBase
    {
        private readonly VideosRepository _repository;

        public VideosController(VideosRepository videosRepository)
        {
            _repository = videosRepository;
        }

        [HttpGet("version")]
        public Rsp<SemVersion> GetVersion()
        {
            return new SemVersion(1, 3, 0);
        }

        [HttpGet("config")]
        public Rsp<Configuration> GetConfiguration()
        {
            return new Configuration
            {
                SiteName = "Sample Site",
                SiteLogo = "https://sample-videos.com/img/Sample-png-image-100kb.png",

                Actors = false,
                Categories = true,
                CategoriesGroups = false,
                Studios = false,
                Analytics = false,
                Scripts = false,
                Masks = false,
                Auth = false,
                NSFW = false,
                Deals = false,

                Theme = 0,
            };
        }

        [HttpGet("videos")]
        public async Task<Rsp<Page<VideoListView>>> GetVideos(
            [FromQuery(Name = "page-index")] long pageIndex,
            [FromQuery(Name = "page-size")] long pageSize,
            [FromQuery(Name = "title")] string searchTitle,
            [FromQuery(Name = "studio")] string studioId,
            [FromQuery(Name = "actor")] string actiorId,
            [FromQuery(Name = "included-categories")] string includedCategories,
            [FromQuery(Name = "excluded-categories")] string excludedCategories,
            [FromQuery(Name = "order")] string sortOrder,
            [FromQuery(Name = "direction")] string sortDirection
            )
        {
            var query = new VideosQuery()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchTitle = searchTitle,
                StudioId = studioId,
                ActorId = actiorId,
                SortDirection = sortDirection,
                SortOrder = sortOrder,
            };

            query.AddIncludedCategories(includedCategories?.Split(',', StringSplitOptions.RemoveEmptyEntries));
            query.AddExcludedCategories(excludedCategories?.Split(',', StringSplitOptions.RemoveEmptyEntries));

            var videos = await _repository.GetVideosAsync(query);
            return videos;
        }

        [HttpGet("video/{videoId}")]
        public async Task<Rsp<VideoView>> GetVideoDetails([FromRoute(Name = "videoId")] string videoId)
        {
            var video = await _repository.GetVideoAsync(videoId);
            return video;
        }

        [HttpGet("categories")]
        public async Task<Rsp<List<CategoryListView>>> GetCategories()
        {
            var categories = await _repository.GetCategoriesAsync();
            return categories;
        }

        [HttpPut("event")]
        public async Task<Rsp> Event([FromBody] JObject parameters)
        {
            const string VIDEO_STREAM_END = "videoStreamEnd";
            const string VIDEO_DOWNLOADED = "videoDownloaded";

            var eventType = parameters.Value<string>("event_type");
            switch (eventType)
            {
                case VIDEO_STREAM_END:
                {
                    var eventData = parameters.ToObject<VideoStreamEnd>();
                    break;
                }
                case VIDEO_DOWNLOADED:
                {
                    var eventData = parameters.ToObject<VideoDownloaded>();
                    break;
                }
            }
            return ApiStatus.OK;
        }
    }
}
