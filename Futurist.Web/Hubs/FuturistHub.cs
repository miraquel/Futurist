using Futurist.Service.Interface;
using Microsoft.AspNetCore.SignalR;

namespace Futurist.Web.Hubs;

public class FuturistHub : Hub
{
    private readonly IMupService _mupService;

    public FuturistHub(IMupService mupService)
    {
        _mupService = mupService;
    }

    // subscribe to IEnumerable<int> MupInProcessRoomIds();
    public async Task SubscribeToMupInProcessRoomIds()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "MupInProcessRoomIds");
        
        await Clients.Caller.SendAsync("MupInProcessRoomIds", _mupService.MupInProcessRoomIds());
    }
}