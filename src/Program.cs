using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using PlayaApiV2.Model;
using PlayaApiV2.Repositories;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddControllers()
            .AddNewtonsoftJson(o =>
            {
                var serializerSettings = o.SerializerSettings;
                serializerSettings.Formatting = Formatting.None;
                serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                serializerSettings.Converters.Add(new SemVersionConverter());
            });

        builder.Services.AddSingleton<VideosRepository>();

        var app = builder.Build();

        app.UseRouting();

        app.MapControllers();

        app.Run();
    }
}