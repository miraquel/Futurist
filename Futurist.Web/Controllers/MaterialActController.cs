using Futurist.Service.Dto;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize(Roles = "costing,sc,admin")]
[Route("[controller]/[action]")]
public class MaterialActController : Controller
{
    private readonly IMaterialActService _materialActService;

    public MaterialActController(IMaterialActService materialActService)
    {
        _materialActService = materialActService;
    }

    public IActionResult Process()
    {
        return View();
    }
}

[ApiController]
[Authorize(Roles = "costing,sc,admin")]
[Route("api/[controller]/[action]")]
public class MaterialActApiController : ControllerBase
{
    private readonly IMaterialActService _materialActService;

    public MaterialActApiController(IMaterialActService materialActService)
    {
        _materialActService = materialActService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetYears()
    {
        var response = await _materialActService.GetMaterialActYearsAsync();
        return Ok(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetMonths([FromQuery] int year)
    {
        var response = await _materialActService.GetMaterialActMonthsAsync(year);
        return Ok(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessMaterialAct([FromBody] ProcessMaterialActRequestDto request)
    {
        return Ok(_materialActService.ProcessMaterialActJob(request.Year, request.Month));
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public IActionResult MaterialActInProcessYearMonths()
    {
        return Ok(_materialActService.MaterialActInProcessYearMonths());
    }
}
