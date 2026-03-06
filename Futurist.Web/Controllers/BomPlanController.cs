using Futurist.Service.Dto;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize(Roles = "costing,sc,admin")]
[Route("[controller]/[action]")]
public class BomPlanController : Controller
{
    public IActionResult Process()
    {
        return View();
    }
}

[ApiController]
[Authorize(Roles = "costing,sc,admin")]
[Route("api/[controller]/[action]")]
public class BomPlanApiController : ControllerBase
{
    private readonly IBomPlanService _bomPlanService;

    public BomPlanApiController(IBomPlanService bomPlanService)
    {
        _bomPlanService = bomPlanService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetRoomIds()
    {
        var response = await _bomPlanService.GetBomPlanRoomIdsAsync();
        return Ok(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetVerIds(int roomId)
    {
        var response = await _bomPlanService.GetBomPlanVerIdsAsync(roomId);
        return Ok(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetYears(int roomId, int verId)
    {
        var response = await _bomPlanService.GetBomPlanYearsAsync(roomId, verId);
        return Ok(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetMonths(int roomId, int verId, int year)
    {
        var response = await _bomPlanService.GetBomPlanMonthsAsync(roomId, verId, year);
        return Ok(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessBomPlan([FromBody] ProcessBomPlanRequestDto request)
    {
        return Ok(_bomPlanService.ProcessBomPlanJob(request.RoomId, request.VerId, request.Year, request.Month));
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public IActionResult BomPlanInProcessRoomIds()
    {
        return Ok(_bomPlanService.BomPlanInProcessRoomIds());
    }
}
