using FlixNet.Services.DTO;
using FlixNet.Services.ServiceInterfaces;
using MongoDB.Driver;

namespace FlixNet.Services
{
    /// <summary>
    /// Class that gets video info and handles upload and streaming through Repository
    /// </summary>
    public class VideoService : IVideoService
    {
        private readonly IMongoDatabase _iMongoDatabase;

        public VideoService(IMongoDatabase mongoDatabase) 
        {
            _iMongoDatabase = mongoDatabase; //Dependency injection to get IMongoDatabase
        }

        // Gets video metadata from MOngoDB
        public async Task<ICollection<VideoDisplayModelDTO>> GetVideoDisplayInfoAsync()
        {

            var videoInfo = await _iMongoDatabase.GetVideoDisplayInfoAsync();

            return videoInfo;
        }


        // Method to get streaming data from MongoDb
        // Method to upload videos til MongoDb
    }
}
