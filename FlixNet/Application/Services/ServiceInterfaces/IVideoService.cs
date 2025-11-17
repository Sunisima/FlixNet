using FlixNet.Application.DTO;

namespace FlixNet.Application.Services.ServiceInterfaces
{
    /// <summary>
    /// Interface to retrieve and manage video data
    /// </summary>
    public interface IVideoService
    {

        Task<ICollection<VideoDisplayDTO>> GetVideoDisplayInfoAsync();
        Task UploadVideoToDatabaseAsync();
    }
}
