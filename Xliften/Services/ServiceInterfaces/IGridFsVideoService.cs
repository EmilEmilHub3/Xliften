using Xliften.Models;

namespace Xliften.Services.ServiceInterfaces
{
    public interface IGridFsVideoService
    {
        Task<(Stream Stream, string ContentType)> GetVideoByIdAsync(string fileId);
        Task<IReadOnlyList<VideoInfo>> GetAllVideosAsync();
    }
}