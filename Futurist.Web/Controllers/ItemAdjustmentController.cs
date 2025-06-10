using Futurist.Common.Helpers;
using Futurist.Service.Command.ItemAdjustmentCommand;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
public class ItemAdjustmentController : Controller
{
    private readonly IItemAdjustmentService _itemAdjustmentService;

    public ItemAdjustmentController(IItemAdjustmentService itemAdjustmentService)
    {
        _itemAdjustmentService = itemAdjustmentService;
    }
    
    public async Task<IActionResult> Index()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var response = await _itemAdjustmentService.GetItemAdjustmentRoomIdsAsync();
        
        if (response.IsSuccess)
        {
            ViewBag.RoomIds = response.Data;
        }
        else
        {
            ViewBag.Error = response.ErrorMessage;
        }
        
        return View();
    }

    public async Task<IActionResult> ExportItemAdjustment([FromQuery] int room)
    {
        var response = await _itemAdjustmentService.GetItemAdjustmentListAsync(room);
        
        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);
        
        var stream = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Item Id";
                row.Cell(3).Value = "Item Name";
                row.Cell(4).Value = "Unit";
                row.Cell(5).Value = "Item Group";
                row.Cell(6).Value = "Group Procurement";
                row.Cell(7).Value = "Price";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.ItemId;
                row.Cell(3).Value = dto.ItemName;
                row.Cell(4).Value = dto.UnitId;
                row.Cell(5).Value = dto.ItemGroup;
                row.Cell(6).Value = dto.GroupProcurement;
                row.Cell(7).Value = dto.Price;
                row.Cell(7).Style.NumberFormat.Format = "#,##0";
            }
        });
        
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ItemAdjustment_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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
        var response = await _itemAdjustmentService.ImportAsync(command);
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
}

[ApiController]
[Authorize]
[ValidateAntiForgeryToken]
[Route("api/[controller]/[action]")]
public class ItemAdjustmentApiController : ControllerBase
{
    private readonly IItemAdjustmentService _itemAdjustmentService;

    public ItemAdjustmentApiController(IItemAdjustmentService itemAdjustmentService)
    {
        _itemAdjustmentService = itemAdjustmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetItemAdjustmentList([FromQuery] int room)
    {
        var result = await _itemAdjustmentService.GetItemAdjustmentListAsync(room);
        
        if (!result.IsSuccess) return BadRequest(result);
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetItemAdjustmentRoomIds()
    {
        var result = await _itemAdjustmentService.GetItemAdjustmentRoomIdsAsync();
        
        if (!result.IsSuccess) return BadRequest(result);
        
        return Ok(result);
    }
}