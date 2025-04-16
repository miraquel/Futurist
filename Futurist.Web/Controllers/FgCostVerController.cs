using Futurist.Repository.Command.FgCostVerCommand;
using Futurist.Service.Interface;
using Futurist.Web.Requests.FgCostVer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class FgCostVerController : Controller
{
    private readonly IFgCostVerService _fgCostVerService;

    public FgCostVerController(IFgCostVerService fgCostVerService)
    {
        _fgCostVerService = fgCostVerService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var response = await _fgCostVerService.GetFgCostVerRoomIdsAsync();
        
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

    [HttpGet]
    public async Task<IActionResult> Process()
    {
        var response = await _fgCostVerService.GetFgCostVerRoomIdsAsync();
        
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
public class FgCostVerApiController : ControllerBase
{
    private readonly IFgCostVerService _fgCostVerService;

    public FgCostVerApiController(IFgCostVerService fgCostVerService)
    {
        _fgCostVerService = fgCostVerService;
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetAllFgCostVer(int roomId)
    {
        var result = await _fgCostVerService.GetAllFgCostVerAsync(roomId);
        return Ok(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InsertFgCostVer([FromBody] InsertFgCostVerRequest request)
    {
        if (request.RoomId <= 0 || string.IsNullOrWhiteSpace(request.Notes))
        {
            return BadRequest(new { title = "Invalid input data." });
        }

        var result = await _fgCostVerService.InsertFgCostVerAsync(request.RoomId, request.Notes);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { title = string.Join(", ", result.Errors) });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFgCostVerRoomIds()
    {
        var result = await _fgCostVerService.GetFgCostVerRoomIdsAsync();
        return Ok(result);
    }
}