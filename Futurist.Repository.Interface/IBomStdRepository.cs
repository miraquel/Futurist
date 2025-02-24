using Futurist.Domain;

namespace Futurist.Repository.Interface;

public interface IBomStdRepository
{
    Task<string?> ProcessBomStdAsync(int roomId);
    Task<IEnumerable<BomStd>> BomErrorCheckAsync(int roomId);
    Task<IEnumerable<int>> GetRoomIdsAsync();
}