using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class MupController : Controller
{
    private readonly IMupService _mupService;

    public MupController(IMupService mupService)
    {
        _mupService = mupService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var response = await _mupService.GetRoomIdsAsync();
        
        if (response.IsSuccess)
        {
            ViewBag.RoomIds = response.Data;
        }
        else
        {
            ViewBag.Error = response.ErrorMessage;
        }
        
        return View();
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class MupApiController : ControllerBase
{
    private readonly IMupService _mupService;

    public MupApiController(IMupService mupService)
    {
        _mupService = mupService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessMup([FromQuery] int roomId)
    {
        var response = await _mupService.ProcessMupAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response.Message);
        }
        
        return BadRequest(response.Errors);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupResult([FromQuery] int roomId)
    {
        var response = await _mupService.MupResultAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        
        return BadRequest(response.Errors);
    }
}