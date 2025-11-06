using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Xliften.Services.ServiceInterfaces;

namespace Xliften.Endpoints
{
    /// <summary>
    /// Endpoint-layer for video-relaterede endpoints i Xliften (Minimal API).
    /// </summary>
    public static class VideoEndpoints
    {
        /// <summary>
        /// Registrerer video-endpoints på app'en.
        /// </summary>
        public static IEndpointRouteBuilder MapVideoEndpoints(this IEndpointRouteBuilder app)
        {
            // GET /video/{id}  -> streamer én video
            app.MapGet("/video/{id}", async (string id, IGridFsVideoService videoService) =>
            {
                try
                {
                    var (stream, contentType) = await videoService.GetVideoByIdAsync(id);
                    return Results.File(stream, contentType);
                }
                catch (FileNotFoundException)
                {
                    return Results.NotFound($"No video found with id {id}");
                }
            })
            .WithName("StreamVideo")
            .WithSummary("Streamer en video direkte fra MongoDB GridFS.")
            .WithDescription("Brug ObjectId fra fs.files collection som id for at streame videoen.");

            // GET /videos  -> returnerer liste af videoer (id + title)
            app.MapGet("/videos", async (IGridFsVideoService videoService) =>
            {
                var videos = await videoService.GetAllVideosAsync();
                return Results.Ok(videos);
            })
            .WithName("VideoInfo")
            .WithSummary("Henter metadata for alle videoer fra MongoDB GridFS.")
            .WithDescription("Returnerer liste af videoer med id og title (filnavn), så frontend kan vælge film.");

            return app;
        }
    }
}
