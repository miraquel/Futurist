using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize(Roles = "admin,costing")]
public class JobMonitoringController : Controller
{
    // GET
    public IActionResult SucceededJobs()
    {
        return View();
    }

    public IActionResult Index()
    {
        return View();
    }
}

[ApiController]
[Authorize(Roles = "admin,costing")]
[Route("api/[controller]/[action]")]
public class JobMonitoringApiController : ControllerBase
{
    private readonly IJobMonitoringService _jobMonitoringService;

    public JobMonitoringApiController(IJobMonitoringService jobMonitoringService)
    {
        _jobMonitoringService = jobMonitoringService;
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetSucceededJobMonitoring([FromQuery] int id)
    {
        var response = await _jobMonitoringService.GetJobMonitoringAsync(id);
        
        if (!response.IsSuccess) return BadRequest(response.Errors);
        
        return Ok(response.Data);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetSucceededJobMonitoringPagedList([FromQuery] PagedListRequestDto pagedListRequestDto)
    {
        var response = await _jobMonitoringService.GetJobMonitoringPagedListAsync(pagedListRequestDto);
        
        if (!response.IsSuccess) return BadRequest(response);
        
        return Ok(response);
    }
}