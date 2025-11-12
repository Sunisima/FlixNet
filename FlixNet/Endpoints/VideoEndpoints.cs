using FlixNet.Services.ServiceInterfaces;

namespace FlixNet.Endpoints
{
    public static class VideoEndpoints
    {
        public static IEndpointRouteBuilder MapVideoEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/videos", async (IVideoService videoService) =>
            {
                var videos = await videoService.GetVideoDisplayInfoAsync();
                return Results.Ok(videos);
            })
            .WithName("GetVideoInfo")
            .WithSummary("Get's meta data from videos saved to return video title, video id and video length.");

            endpoints.MapGet("/video/{id}", async (string id, IVideoService videoService) =>
            {
                try
                {
                    // Return the video stream
                    return Results.Ok();
                }
                catch (FileNotFoundException)
                {
                    return Results.NotFound();
                }
                catch (Exception e)
                {
                    return Results.Problem(e.Message);
                }
            })
            .WithName("GetVideoStream")
            .WithSummary("Get's a stream of the video with provided id");

            return endpoints;
        }
    }
}
