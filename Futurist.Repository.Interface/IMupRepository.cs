using System.Data;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.MupCommand;

namespace Futurist.Repository.Interface;

public interface IMupRepository
{
    Task<SpTask?> ProcessMupAsync(ProcessMupCommand command);
    Task<IEnumerable<MupSp>> MupResultAsync(MupResultCommand command);
    Task<PagedList<MupSp>> MupResultPagedListAsync(MupResultPagedListCommand command);
    Task<IEnumerable<MupSp>> MupSummaryByItemIdFromSpAsync(MupSummaryByItemIdFromSpCommand command);
    Task<IEnumerable<MupSp>> MupSummaryByItemIdAsync(MupSummaryByItemIdCommand command);
    Task<PagedList<MupSp>> MupSummaryByItemIdPagedListAsync(MupSummaryByItemIdPagedListCommand command);
    Task<IEnumerable<MupSp>> MupSummaryByBatchNumberFromSpAsync(MupSummaryByBatchNumberFromSpCommand command);
    Task<IEnumerable<MupSp>> MupSummaryByBatchNumberAsync(MupSummaryByBatchNumberCommand command);
    Task<PagedList<MupSp>> MupSummaryByBatchNumberPagedListAsync(MupSummaryByBatchNumberPagedListCommand command);
    Task<IEnumerable<int>> GetMupRoomIdsAsync(GetMupRoomIdsCommand command);
}