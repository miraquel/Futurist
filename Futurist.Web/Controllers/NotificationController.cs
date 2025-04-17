using Futurist.Infrastructure.SignalR.Hubs;
using Futurist.Service.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Futurist.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class NotificationController : ControllerBase
{
    private readonly IHubContext<FuturistHub> _hubContext;

    public NotificationController(IHubContext<FuturistHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Notify([FromBody] NotificationDto<IEnumerable<int>> notificationDto)
    {
        var method = notificationDto.Method;
        var roomIds = notificationDto.Data ?? [];

        if (string.IsNullOrEmpty(method))
        {
            return BadRequest("Invalid parameters");
        }

        await _hubContext.Clients.All.SendAsync(method, roomIds);
        return Ok("Notification sent successfully");
    }
}