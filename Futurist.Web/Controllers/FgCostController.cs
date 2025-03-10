using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class FgCostController : Controller
{
    private readonly IFgCostService _fgCostService;

    public FgCostController(IFgCostService fgCostService)
    {
        _fgCostService = fgCostService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var response = await _fgCostService.GetFgCostRoomIdsAsync();
        
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
    
    public async Task<IActionResult> Details()
    {
        var response = await _fgCostService.GetFgCostRoomIdsAsync();
        
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
    
    public async Task<IActionResult> Process()
    {
        var response = await _fgCostService.GetFgCostRoomIdsAsync();
        
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
public class FgCostApiController : ControllerBase
{
    private readonly IFgCostService _fgCostService;

    public FgCostApiController(IFgCostService fgCostService)
    {
        _fgCostService = fgCostService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetSummaryFgCostPagedListAsync([FromQuery] PagedListRequestDto<FgCostSpDto> dto)
    {
        var response = await _fgCostService.GetSummaryFgCostPagedListAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetFgCostDetailPagedListAsync([FromQuery] PagedListRequestDto<FgCostDetailSpDto> dto)
    {
        var response = await _fgCostService.GetFgCostDetailPagedListAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CalculateFgCostAsync([FromQuery] int roomId)
    {
        var response = await _fgCostService.CalculateFgCostAsync(roomId);

        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        return BadRequest(response.ErrorMessage);
    }
}