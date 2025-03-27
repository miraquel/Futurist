using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.FgCostCommand;

namespace Futurist.Repository.Interface;

public interface IFgCostRepository
{
    public Task<SpTask?> CalculateFgCostAsync(CalculateFgCostCommand command);
    public Task<IEnumerable<FgCostSp>> GetSummaryFgCostAsync(GetSummaryFgCostCommand command);
    public Task<PagedList<FgCostSp>> GetSummaryFgCostPagedListAsync(GetSummaryFgCostPagedListCommand command);
    public Task<IEnumerable<int>> GetFgCostRoomIdsAsync(GetFgCostRoomIdsCommand command);
    
    public Task<IEnumerable<FgCostDetailSp>> GetFgCostDetailsAsync(GetFgCostDetailCommand command);
    public Task<PagedList<FgCostDetailSp>> GetFgCostDetailsPagedListAsync(GetFgCostDetailPagedListCommand command);
    public Task<IEnumerable<FgCostDetailSp>> GetFgCostDetailsByRofoIdAsync(GetFgCostDetailsByRofoIdCommand command);
    public Task<PagedList<FgCostDetailSp>> GetFgCostDetailsByRofoIdPagedListAsync(GetFgCostDetailsByRofoIdPagedListCommand command);
}