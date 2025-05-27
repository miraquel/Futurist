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

[Authorize]
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
        var stream = ExcelHelper.CreateExcelTemplate(list =>
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(nameof(ExchangeRateSpDto.CurrencyCode));
            list.Add(nameof(ExchangeRateSpDto.ValidFrom));
            list.Add(nameof(ExchangeRateSpDto.ValidTo));
            list.Add(nameof(ExchangeRateSpDto.ExchangeRate));
        });
        
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExchangeRateTemplate.xlsx");
    }
    
    public async Task<IActionResult> DownloadExchangeRate()
    {
        var response = await _exchangeRateService.GetAllExchangeRateAsync();

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);

        var result = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Currency";
                row.Cell(2).Value = "Valid From";
                row.Cell(3).Value = "Valid To";
                row.Cell(4).Value = "Exchange Rate";
            }
            else
            {
                row.Cell(1).Value = dto.CurrencyCode;
                row.Cell(2).Value = dto.ValidFrom;
                row.Cell(2).Style.DateFormat.Format = "dd MMM yyyy";
                row.Cell(3).Value = dto.ValidTo;
                row.Cell(3).Style.DateFormat.Format = "dd MMM yyyy";
                row.Cell(4).Value = dto.ExchangeRate;
                row.Cell(4).Style.NumberFormat.Format = "#,##0";
            }
        });

        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"FgCostDetail_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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