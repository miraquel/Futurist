using Futurist.Service.Dto;

namespace Futurist.Repository.Interface;

public interface IMupRepository
{
    Task<IEnumerable<MupSp>> ProcessMupAsync(int roomId);
    Task<IEnumerable<MupSp>> MupResultAsync(int roomId);
    Task<IEnumerable<int>> GetRoomIdsAsync();
}