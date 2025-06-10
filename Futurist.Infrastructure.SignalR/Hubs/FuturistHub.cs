using Microsoft.AspNetCore.SignalR;
using Futurist.Service.Interface;

namespace Futurist.Infrastructure.SignalR.Hubs;

public class FuturistHub : Hub
{
    private readonly IMupService _mupService;
    private readonly IBomStdService _bomStdService;
    private readonly IFgCostService _fgCostService;

    public FuturistHub(IMupService mupService, IBomStdService bomStdService, IFgCostService fgCostService)
    {
        _mupService = mupService;
        _bomStdService = bomStdService;
        _fgCostService = fgCostService;
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
}