using FlixNet.Services.DTO;
using FlixNet.Services.ServiceInterfaces;

namespace FlixNet.Services
{
    /// <summary>
    /// Class that gets video info and handles upload and streaming through Repository
    /// </summary>
    public class VideoService
    {
        private readonly IVideoRepository _videoRepository;

        public VideoService(IVideoRepository videoRepository) 
        {
            _videoRepository = videoRepository; //DI
        }

        // Gets video metadata
        public async Task<ICollection<VideoDisplayModelDTO>> GetVideoDisplayInfoAsync()
        {

            var videoInfo = await _videoRepository.GetVideoDisplayInfoAsync();

            return videoInfo;
        }

        // Method to get streaming data from MongoDb
        // Method to upload videos til MongoDb
        // Lav connection klasse til MongoDb.
    }
}
