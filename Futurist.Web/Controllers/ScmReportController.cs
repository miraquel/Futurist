using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
public class ScmReportController : Controller
{
    private readonly IScmReportService _scmReportService;

    public ScmReportController(IScmReportService scmReportService)
    {
        _scmReportService = scmReportService;
    }

    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticByProductCustomer()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
    
    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticByProduct()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
    
    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticByCustomer()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
    
    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticRawData()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
    
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportByProductCustomer()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
    
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportByProduct()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
    
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportByCustomer()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
    
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportRawData()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var years = await _scmReportService.GetYearsAsync();
        
        ViewBag.Years = years;
        
        return View();
    }
}

[Route("api/[controller]/[action]")]
[ApiController]
[ValidateAntiForgeryToken]
[Authorize]
public class ScmReportApiController : ControllerBase
{
    private readonly IScmReportService _scmReportService;

    public ScmReportApiController(IScmReportService scmReportService)
    {
        _scmReportService = scmReportService;
    }
    
    // get months by year
    [HttpGet]
    public async Task<IActionResult> GetMonths([FromQuery] int year)
    {
        try
        {
            var months = await _scmReportService.GetMonthsAsync(year);
            return Ok(months);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                Message = "Error retrieving months",
                Error = e.Message
            });
        }
    }
    
    // GetDomesticByProductCustomer
    [HttpGet]
    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticByProductCustomer([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetDomesticByProductCustomer(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    // GetDomesticByProduct
    [HttpGet]
    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticByProduct([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetDomesticByProduct(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    // GetDomesticByCustomer
    [HttpGet]
    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticByCustomer([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetDomesticByCustomer(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    // GetDomesticRawData
    [HttpGet]
    [Authorize(Roles = "costing,marketing_domestik,admin")]
    public async Task<IActionResult> GetDomesticRawData([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetDomesticRawData(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    // GetExportByProductCustomer
    [HttpGet]
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportByProductCustomer([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetExportByProductCustomer(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    // GetExportByProduct
    [HttpGet]
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportByProduct([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetExportByProduct(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    // GetExportByCustomer
    [HttpGet]
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportByCustomer([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetExportByCustomer(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    // GetExportRawData
    [HttpGet]
    [Authorize(Roles = "costing,marketing_export,admin")]
    public async Task<IActionResult> GetExportRawData([FromQuery] int year, [FromQuery] int month)
    {
        var periodeDate = new DateTime(year, month, 1);
        var response = await _scmReportService.GetExportRawData(periodeDate);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}