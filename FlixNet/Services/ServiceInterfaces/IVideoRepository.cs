using FlixNet.Services.DTO;
using Microsoft.AspNetCore.SignalR;

namespace FlixNet.Services.ServiceInterfaces
{
    /// <summary>
    /// Interface to retrieve and manage video data
    /// </summary>
    public interface IVideoRepository
    {

        public Task<ICollection<VideoDisplayModelDTO>> GetVideoDisplayInfoAsync();
    }
}
