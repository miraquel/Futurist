using Futurist.Common.Helpers;
using Futurist.Service.Command.RofoCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
public class RofoController : Controller
{
    private readonly IRofoService _rofoService;

    public RofoController(IRofoService rofoService)
    {
        _rofoService = rofoService;
    }

    // GET: RofoController with pagination
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
        
        var response = await _rofoService.GetRofoRoomIdsAsync();
        
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

    // POST: RofoController/Create
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
            User = User.FindFirst("preferred_username")?.Value ?? "Unknown",
        };
        var response = await _rofoService.ImportAsync(command);
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
            list.Add(nameof(RofoDto.Room));
            list.Add(nameof(RofoDto.RofoDate));
            list.Add(nameof(RofoDto.ItemId));
            list.Add(nameof(RofoDto.ItemName));
            list.Add(nameof(RofoDto.Qty));
        });
        
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RofoTemplate.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportRofo([FromQuery] int room)
    {
        var response = await _rofoService.GetRofoListAsync(room);
        
        if (!response.IsSuccess) return BadRequest(response.Errors);

        if (response.Data == null)
        {
            return BadRequest("No data found");
        }

        var stream = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Rofo Date";
                row.Cell(3).Value = "Item Id";
                row.Cell(4).Value = "Item Name";
                row.Cell(5).Value = "Qty";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.RofoDate;
                row.Cell(2).Style.NumberFormat.Format = "dd MMM yyyy";
                row.Cell(3).Value = dto.ItemId;
                row.Cell(4).Value = dto.ItemName;
                row.Cell(5).Value = dto.Qty;
                row.Cell(5).Style.NumberFormat.Format = "#,##0";
            }
        });

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Rofo_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class RofoApiController : ControllerBase
{
    private readonly IRofoService _rofoService;

    public RofoApiController(IRofoService rofoService)
    {
        _rofoService = rofoService;
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetRofoPagedList([FromQuery] PagedListRequestDto pagedListRequest)
    {
        var response = await _rofoService.GetPagedListAsync(pagedListRequest);
        return Ok(response);
    }
}