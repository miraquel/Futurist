using System.Data;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.MupCommand;

namespace Futurist.Repository.Interface;

public interface IMupRepository
{
    Task<IEnumerable<MupSp>> ProcessMupAsync(ProcessMupCommand command);
    Task<IEnumerable<MupSp>> MupResultAsync(MupResultCommand command);
    Task<PagedList<MupSp>> MupResultPagedListAsync(MupResultPagedListCommand command);
    Task<IEnumerable<int>> GetMupRoomIdsAsync(GetMupRoomIdsCommand command);
}