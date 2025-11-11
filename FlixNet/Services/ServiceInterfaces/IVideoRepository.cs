using FlixNet.Services.DTO;
using Microsoft.AspNetCore.SignalR;

namespace FlixNet.Services.ServiceInterfaces
{
    public interface IVideoRepository
    {

        public Task<ICollection<VideoDisplayModelDTO>> GetVideoDisplayInfoAsync();
    }
}
