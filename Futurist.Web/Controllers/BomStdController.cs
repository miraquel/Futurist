using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class BomStdController : Controller
{
    private readonly IBomStdService _bomStdService;

    public BomStdController(IBomStdService bomStdService)
    {
        _bomStdService = bomStdService;
    }
    
    public async Task<IActionResult> Index()
    {
        var response = await _bomStdService.GetRoomIdsAsync();
        
        if (response.IsSuccess)
        {
            ViewBag.RoomIds = response.Data;
        }
        else
        {
            ViewBag.Error = string.Join(", ", response.Errors);
        }
        
        return View();
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class BomStdApiController : ControllerBase
{
    private readonly IBomStdService _bomStdService;

    public BomStdApiController(IBomStdService bomStdService)
    {
        _bomStdService = bomStdService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BomErrorCheck([FromQuery] int roomId)
    {
        var response = await _bomStdService.BomErrorCheckAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        
        return BadRequest(response.Errors);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessBomStd([FromQuery] int roomId)
    {
        var response = await _bomStdService.ProcessBomStdAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response.Message);
        }
        
        return BadRequest(response.Errors);
    }
}