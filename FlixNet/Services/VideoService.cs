using FlixNet.Services.DTO;
using FlixNet.Services.ServiceInterfaces;

namespace FlixNet.Services
{
    /// <summary>
    /// Class to get info for the dropdownlist, get video data and upload videoes to MongoDB
    /// </summary>
    public class VideoService
    {
        private readonly IVideoRepository _videoRepository;

        public VideoService(IVideoRepository videoRepository) 
        {
            _videoRepository = videoRepository;
        }

        // Displays info to the dropdown list
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
