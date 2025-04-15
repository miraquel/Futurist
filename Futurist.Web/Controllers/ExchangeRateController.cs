using System.ComponentModel;
using System.Reflection;
using Futurist.Common.Helpers;
using Futurist.Service.Command.ExchangeRateCommand;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Futurist.Service.Dto.Common;
using Futurist.Service.Dto;

namespace Futurist.Web.Controllers;

public class ExchangeRateController : Controller
{
    private readonly IExchangeRateService _exchangeRateService;

    public ExchangeRateController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    // GET
    public IActionResult Index()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Import([FromForm(Name = "file")] IFormFile fileInput)
    {
        if (fileInput.Length == 0)
        {
            return RedirectToAction(nameof(Index));
        }
        var command = new ImportCommand
        {
            Stream = fileInput.OpenReadStream(),
            User = User.FindFirst("preferred_username")?.Value ?? "Unknown"
        };
        var response = await _exchangeRateService.ImportAsync(command);
        if (response.IsSuccess)
        {
            TempData["Success"] = response.Message;
        }
        else
        {
            TempData["Errors"] = response.Errors;
        }
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        var stream = ExcelHelper.CreateExcelTemplate<ExchangeRateSpDto>(list =>
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(nameof(ExchangeRateSpDto.CurrencyCode));
            list.Add(nameof(ExchangeRateSpDto.ValidFrom));
            list.Add(nameof(ExchangeRateSpDto.ValidTo));
            list.Add(nameof(ExchangeRateSpDto.ExchangeRate));
        });
        
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExchangeRateTemplate.xlsx");
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class ExchangeRateApiController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;

    public ExchangeRateApiController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetExchangeRatePagedList([FromQuery] PagedListRequestDto pagedListRequest)
    {
        var response = await _exchangeRateService.GetExchangeRatePagedListAsync(pagedListRequest);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
}