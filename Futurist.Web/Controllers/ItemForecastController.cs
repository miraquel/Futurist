using Futurist.Common.Helpers;
using Futurist.Service.Command.ItemForecastCommand;
using Futurist.Service.Dto;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize(Roles = "costing,sc,admin")]
public class ItemForecastController : Controller
{
    private readonly IItemForecastService _service;

    public ItemForecastController(IItemForecastService service)
    {
        _service = service;
    }

    // GET
    public async Task<IActionResult> Index([FromQuery] int room = 0)
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }

        var response = await _service.GetItemForecastRoomIdsAsync();
        
        if (response.IsSuccess)
        {
            ViewBag.RoomIds = response.Data;
        }
        else
        {
            ViewBag.Error = response.ErrorMessage;
        }
        
        if (room > 0)
        {
            ViewBag.InitialRoomId = room;
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import([FromForm(Name = "file")] IFormFile fileInput, [FromQuery] int room)
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

        var response = await _service.ImportAsync(command);

        if (response.IsSuccess)
        {
            TempData["Success"] = response.Message;
        }
        else
        {
            TempData["Errors"] = response.Errors;
        }

        return RedirectToAction(nameof(Index), new { room = response.Data > 0 ? response.Data : room });
    }
    
    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        var stream = ExcelHelper.CreateExcelTemplate(list =>
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(nameof(ItemForecastSpDto.Room));
            list.Add(nameof(ItemForecastSpDto.ItemId));
            list.Add(nameof(ItemForecastSpDto.ForecastDate));
            list.Add(nameof(ItemForecastSpDto.ForcedPrice));
        });
        
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ItemForecastTemplate.xlsx");
    }
    
    [HttpGet]
    public async Task<IActionResult> ExportItemForecast([FromQuery] int room)
    {
        var response = await _service.GetItemForecastListAsync(room);
        
        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);
        
        var stream = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Item Id";
                row.Cell(3).Value = "Item Name";
                row.Cell(4).Value = "Unit";
                row.Cell(5).Value = "Group Substitusi";
                row.Cell(6).Value = "Group Procurement";
                row.Cell(7).Value = "Price";
                row.Cell(8).Value = "Latest Purchase Date";
                row.Cell(9).Value = "Forecast Date";
                row.Cell(10).Value = "Forecast Price";
                row.Cell(11).Value = "Forced Price";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.ItemId;
                row.Cell(3).Value = dto.ItemName;
                row.Cell(4).Value = dto.UnitId;
                row.Cell(5).Value = dto.VtaMpSubstitusiGroupId;
                row.Cell(6).Value = dto.GroupProcurement;
                row.Cell(7).Value = dto.Price;
                row.Cell(7).Style.NumberFormat.Format = "#,##0";
                row.Cell(8).Value = dto.LatestPurchaseDate;
                row.Cell(8).Style.DateFormat.Format = "dd MMM yyyy";
                row.Cell(9).Value = dto.ForecastDate;
                row.Cell(9).Style.DateFormat.Format = "dd MMM yyyy";
                row.Cell(10).Value = dto.ForecastPrice;
                row.Cell(10).Style.NumberFormat.Format = "#,##0";
                row.Cell(11).Value = dto.ForcedPrice;
                row.Cell(11).Style.NumberFormat.Format = "#,##0";
            }
        });
        
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ItemForecast_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
    
}

[ApiController]
[Authorize(Roles = "costing,sc,admin")]
[Route("api/[controller]/[action]")]
public class ItemForecastApiController : ControllerBase
{
    private readonly IItemForecastService _itemForecastService;

    public ItemForecastApiController(IItemForecastService itemForecastService)
    {
        _itemForecastService = itemForecastService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetItemForecastList([FromQuery] int room)
    {
        var response = await _itemForecastService.GetItemForecastListAsync(room);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetItemForecastRoomIds()
    {
        var response = await _itemForecastService.GetItemForecastRoomIdsAsync();

        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        return BadRequest(response.Errors);
    }
}