using FlixNet.Application.DTO;
using Microsoft.AspNetCore.SignalR;

namespace FlixNet.Application.Services.ServiceInterfaces
{
    /// <summary>
    /// Interface to retrieve and manage video data
    /// </summary>
    public interface IVideoService
    {

        public Task<ICollection<VideoDisplayModelDTO>> GetVideoDisplayInfoAsync();
    }
}
