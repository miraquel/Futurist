using System.Data;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command;
using Futurist.Repository.Command.BomStdCommand;

namespace Futurist.Repository.Interface;

public interface IBomStdRepository
{
    Task<string?> ProcessBomStdAsync(ProcessBomStdCommand command);
    Task<IEnumerable<BomStd>> BomErrorCheckAsync(BomErrorCheckCommand command);
    Task<PagedList<BomStd>> BomErrorCheckPagedListAsync(BomErrorCheckPagedListCommand command);
    Task<IEnumerable<int>> GetBomStdRoomIdsAsync(GetBomStdRoomIdsCommand command);
}