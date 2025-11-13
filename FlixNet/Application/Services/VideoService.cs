using FlixNet.Application.DTO;
using FlixNet.Application.Services.ServiceInterfaces;
using MongoDB.Driver;

namespace FlixNet.Application.Services
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

        // Gets video metadata from MongoDB
        public async Task<ICollection<VideoDisplayModelDTO>> GetVideoDisplayInfoAsync()
        {
            List<VideoDisplayModelDTO> videoes = new List<VideoDisplayModelDTO>();

            videoes.Add(new VideoDisplayModelDTO
            {
                Id = "1",
                Title = "Hello",
                Duration = TimeSpan.FromMinutes(19)
            });

            videoes.Add(new VideoDisplayModelDTO
            {
                Id = "2",
                Title = "I'm here!",
                Duration = TimeSpan.FromMinutes(31) + TimeSpan.FromSeconds(58)
            });

            return await Task.FromResult(videoes as ICollection<VideoDisplayModelDTO>);
        }


        // Method to get streaming data from MongoDb

        // Method to upload videos til MongoDb
    }
}
