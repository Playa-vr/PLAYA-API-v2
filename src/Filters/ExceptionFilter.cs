using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

using Newtonsoft.Json;

using PlayaApiV2.Model;

using System;
using System.Net;
using System.Threading.Tasks;

namespace PlayaApiV2.Filters
{
    public class ExceptionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                await next();
            }
            catch (Exception error)
            {
                await HandleError(context.HttpContext, error);
            }
        }

        private Task HandleError(HttpContext context, Exception error)
        {
            if (context.Response.HasStarted)
                return Task.CompletedTask;

            return Error(error, context);
        }

        private static Task Error(Exception error, HttpContext context)
        {
            var body = error switch
            {
                ApiException e => new Rsp(ApiStatus.From(message: e.Message, code: e.Code)),
                _ => new Rsp(ApiStatus.From(message: error.Message, code: ApiStatusCodes.ERROR)),
            };
            return WriteAsJsonAsync(context.Response, body);
        }

        private static readonly JsonSerializerSettings s_serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static Task WriteAsJsonAsync<T>(HttpResponse response, T body, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var json = JsonConvert.SerializeObject(body, s_serializerSettings);
            response.StatusCode = (int)statusCode;
            response.ContentType = "application/json; charset=utf-8";
            return response.WriteAsync(json);
        }
    }
}
