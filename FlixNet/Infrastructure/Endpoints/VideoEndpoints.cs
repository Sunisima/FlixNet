using FlixNet.Application.Services.ServiceInterfaces;

namespace FlixNet.Infrastructure.Endpoints
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
                    Stream videoStream =  await videoService.GetVideoStreamByIdAsync(id);
                    return Results.File(videoStream, "video/mp4");
                }
                catch (FileNotFoundException e)
                {
                    return Results.NotFound(e.Message);
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
