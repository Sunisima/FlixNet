using FlixNet.Services.DTO;
using FlixNet.Services.ServiceInterfaces;

namespace FlixNet.Infrastructure.MongoVideoRepository
{
    /// <summary>
    /// Repository that retrieves, stores and streams video data from MongoDb.
    /// </summary>
    public class MongoVideoRepository : IVideoRepository
    {
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
    }
}
