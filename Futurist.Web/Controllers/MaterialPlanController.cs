using Futurist.Service.Dto;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize(Roles = "costing,sc,admin")]
[Route("[controller]/[action]")]
public class MaterialPlanController : Controller
{
    private readonly IMaterialPlanService _materialPlanService;

    public MaterialPlanController(IMaterialPlanService materialPlanService)
    {
        _materialPlanService = materialPlanService;
    }

    public IActionResult Process()
    {
        return View();
    }
}

[ApiController]
[Authorize(Roles = "costing,sc,admin")]
[Route("api/[controller]/[action]")]
public class MaterialPlanApiController : ControllerBase
{
    private readonly IMaterialPlanService _materialPlanService;

    public MaterialPlanApiController(IMaterialPlanService materialPlanService)
    {
        _materialPlanService = materialPlanService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetRoomIds()
    {
        var response = await _materialPlanService.GetMaterialPlanRoomIdsAsync();
        return Ok(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessMaterialPlan([FromBody] ProcessMaterialPlanRequestDto request)
    {
        return Ok(_materialPlanService.ProcessMaterialPlanJob(request.RoomId, request.VerId, request.Year, request.Month));
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetVerIds(int roomId)
    {
        var response = await _materialPlanService.GetMaterialPlanVerIdsAsync(roomId);
        return Ok(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetYears(int roomId, int verId)
    {
        var response = await _materialPlanService.GetMaterialPlanYearsAsync(roomId, verId);
        return Ok(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetMonths(int roomId, int verId, int year)
    {
        var response = await _materialPlanService.GetMaterialPlanMonthsAsync(roomId, verId, year);
        return Ok(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public IActionResult MaterialPlanInProcessRoomIds()
    {
        return Ok(_materialPlanService.MaterialPlanInProcessRoomIds());
    }
}
