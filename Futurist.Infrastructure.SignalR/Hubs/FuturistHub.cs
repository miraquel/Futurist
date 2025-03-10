using Microsoft.AspNetCore.SignalR;
using Futurist.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Futurist.Infrastructure.SignalR.Hubs;

public class FuturistHub : Hub
{
    private readonly IMupService _mupService;
    private readonly IBomStdService _bomStdService;

    public FuturistHub(IMupService mupService, IBomStdService bomStdService)
    {
        _mupService = mupService;
        _bomStdService = bomStdService;
    }
    
    public Task<IEnumerable<int>> GetMupInProcessRoomIds()
    {
        return Task.FromResult(_mupService.MupInProcessRoomIds());
    }
    
    public Task<IEnumerable<int>> GetBomStdInProcessRoomIds()
    {
        return Task.FromResult(_bomStdService.GetBomStdInProcessRoomIds());
    }
}