using Microsoft.AspNetCore.SignalR;
using Futurist.Service.Interface;

namespace Futurist.Infrastructure.SignalR.Hubs;

public class FuturistHub : Hub
{
    private readonly IMupService _mupService;
    private readonly IBomStdService _bomStdService;
    private readonly IFgCostService _fgCostService;
    private readonly IMaterialPlanService _materialPlanService;
    private readonly IMaterialActService _materialActService;
    private readonly IBomPlanService _bomPlanService;

    public FuturistHub(IMupService mupService, IBomStdService bomStdService, IFgCostService fgCostService, IMaterialPlanService materialPlanService, IMaterialActService materialActService, IBomPlanService bomPlanService)
    {
        _mupService = mupService;
        _bomStdService = bomStdService;
        _fgCostService = fgCostService;
        _materialPlanService = materialPlanService;
        _materialActService = materialActService;
        _bomPlanService = bomPlanService;
    }
    
    public Task<IEnumerable<int>> GetMupInProcessRoomIds()
    {
        return Task.FromResult(_mupService.MupInProcessRoomIds());
    }
    
    public Task<IEnumerable<int>> GetBomStdInProcessRoomIds()
    {
        return Task.FromResult(_bomStdService.GetBomStdInProcessRoomIds());
    }
    
    public Task<IEnumerable<int>> GetFgCostInProcessRoomIds()
    {
        return Task.FromResult(_fgCostService.GetFgCostInProcessRoomIds());
    }
    
    public Task<IEnumerable<int>> GetMaterialPlanInProcessRoomIds()
    {
        return Task.FromResult(_materialPlanService.MaterialPlanInProcessRoomIds());
    }
    
    public Task<IEnumerable<string>> GetMaterialActInProcessYearMonths()
    {
        return Task.FromResult(_materialActService.MaterialActInProcessYearMonths());
    }

    public Task<IEnumerable<int>> GetBomPlanInProcessRoomIds()
    {
        return Task.FromResult(_bomPlanService.BomPlanInProcessRoomIds());
    }
}