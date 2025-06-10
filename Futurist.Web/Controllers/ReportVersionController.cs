using Futurist.Common.Helpers;
using Futurist.Service.Interface;
using Futurist.Web.Requests.ReportVersion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize(Roles = "costing,sc,rni,admin,marketing_domestik,marketing_export")]
public class ReportVersionController : Controller
{
    private readonly IReportVersionService _reportVersionService;

    public ReportVersionController(IReportVersionService reportVersionService)
    {
        _reportVersionService = reportVersionService;
    }

    // GET
    public async Task<IActionResult> GetAllFgCostVer()
    {
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
        }
        
        var response = await _reportVersionService.GetVersionRoomIdsAsync();
        
        if (response.IsSuccess)
        {
            ViewBag.RoomIds = response.Data;
        }
        else
        {
            ViewBag.Error = string.Join(", ", response.Errors);
        }
        
        return View();
    }
    
    public IActionResult GetAllFgCostVerDetailsByRofoId([FromQuery] int rofoId, [FromQuery] int verId)
    {
        if (rofoId > 0 && verId > 0)
        {
            ViewBag.RofoId = rofoId;
            ViewBag.VerId = verId;
            return View();
        }
        
        TempData["Errors"] = "Invalid room or Rofo ID.";
        return RedirectToAction(nameof(GetAllFgCostVer));

    }

    [HttpGet]
    [Authorize(Roles = "costing")]
    public async Task<IActionResult> Process()
    {
        var response = await _reportVersionService.GetVersionRoomIdsAsync();
        
        if (response.IsSuccess)
        {
            ViewBag.RoomIds = response.Data;
        }
        else
        {
            ViewBag.Error = string.Join(", ", response.Errors);
        }
        
        return View();
    }

    public async Task<IActionResult> GetAllMupVer()
    {
        var response = await _reportVersionService.GetVersionRoomIdsAsync();
        
        if (response.IsSuccess)
        {
            ViewBag.RoomIds = response.Data;
        }
        else
        {
            ViewBag.Error = string.Join(", ", response.Errors);
        }
        
        return View();
    }
    
    public async Task<IActionResult> DownloadSummaryFgCostVer([FromQuery] int room, [FromQuery] int verId)
    {
        var response = await _reportVersionService.GetAllFgCostVerAsync(room, verId);
    
        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);
        
        var result = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Item ID";
                row.Cell(3).Value = "Item Name";
                row.Cell(4).Value = "Unit";
                row.Cell(5).Value = "In Kg";
                row.Cell(6).Value = "Sales Price";
                row.Cell(7).Value = "Rofo Qty";
                row.Cell(8).Value = "Rofo Date";
                row.Cell(9).Value = "RM Price";
                row.Cell(10).Value = "PM Price";
                row.Cell(11).Value = "Std Cost Price";
                row.Cell(12).Value = "Yield";
                row.Cell(13).Value = "RMPM+Y";
                row.Cell(14).Value = "Cost Price";
                row.Cell(15).Value = "Previous Calc";
                row.Cell(16).Value = "Sales Price Prev";
                row.Cell(17).Value = "Rofo Qty Prev";
                row.Cell(18).Value = "RM Prev";
                row.Cell(18).Value = "PM Prev";
                row.Cell(19).Value = "Std Cost Prev";
                row.Cell(20).Value = "Yield Prev";
                row.Cell(21).Value = "RMPM+Y Prev";
                row.Cell(22).Value = "Cost Price Prev";
                row.Cell(23).Value = "Delta Absolute";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.ProductId;
                row.Cell(3).Value = dto.ProductName;
                row.Cell(4).Value = dto.Unit;
                row.Cell(5).Value = dto.UnitInKg;
                row.Cell(5).Style.NumberFormat.Format = "#,##0";
                row.Cell(6).Value = dto.SalesPrice;
                row.Cell(6).Style.NumberFormat.Format = "#,##0";
                row.Cell(7).Value = dto.RofoQty;
                row.Cell(7).Style.NumberFormat.Format = "#,##0";
                row.Cell(8).Value = dto.RofoDate;
                row.Cell(8).Style.DateFormat.Format = "dd MMM yyyy";
                row.Cell(9).Value = dto.RmPrice;
                row.Cell(9).Style.NumberFormat.Format = "#,##0";
                row.Cell(10).Value = dto.PmPrice;
                row.Cell(10).Style.NumberFormat.Format = "#,##0";
                row.Cell(11).Value = dto.StdCostPrice;
                row.Cell(11).Style.NumberFormat.Format = "#,##0";
                row.Cell(12).Value = dto.Yield;
                row.Cell(12).Style.NumberFormat.Format = "#,##0.0%";
                row.Cell(13).Value = dto.CostRmPmY;
                row.Cell(13).Style.NumberFormat.Format = "#,##0";
                row.Cell(14).Value = dto.CostPrice;
                row.Cell(14).Style.NumberFormat.Format = "#,##0";
                row.Cell(15).Value = dto.PreviousCalc;
                row.Cell(16).Value = dto.SalesPricePrev;
                row.Cell(16).Style.NumberFormat.Format = "#,##0";
                row.Cell(17).Value = dto.RofoQtyPrev;
                row.Cell(17).Style.NumberFormat.Format = "#,##0";
                row.Cell(18).Value = dto.RmPrev;
                row.Cell(18).Style.NumberFormat.Format = "#,##0";
                row.Cell(19).Value = dto.PmPrev;
                row.Cell(19).Style.NumberFormat.Format = "#,##0";
                row.Cell(20).Value = dto.StdCostPrev;
                row.Cell(20).Style.NumberFormat.Format = "#,##0";
                row.Cell(21).Value = dto.YieldPrev;
                row.Cell(21).Style.NumberFormat.Format = "#,##0";
                row.Cell(22).Value = dto.CostRmPmYPrev;
                row.Cell(22).Style.NumberFormat.Format = "#,##0";
                row.Cell(23).Value = dto.CostPricePrev;
                row.Cell(23).Style.NumberFormat.Format = "#,##0";
                row.Cell(24).Value = dto.DeltaAbsolute;
                row.Cell(24).Style.NumberFormat.Format = "#,##0";
            }
        });
        
        // download as excel file
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SummaryFgCost_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}

[ApiController]
[Authorize(Roles = "costing,sc,rni,admin,marketing_domestik,marketing_export")]
[Route("api/[controller]/[action]")]
public class ReportVersionApiController : ControllerBase
{
    private readonly IReportVersionService _reportVersionService;

    public ReportVersionApiController(IReportVersionService reportVersionService)
    {
        _reportVersionService = reportVersionService;
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetAllFgCostVer([FromQuery] int room, [FromQuery] int verId)
    {
        var result = await _reportVersionService.GetAllFgCostVerAsync(room, verId);
        if (result is not { IsSuccess: true, Data: not null })
        {
            return BadRequest(new { title = string.Join(", ", result.Errors) });
        }
        return Ok(result);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetAllFgCostVerDetailsByRofoId([FromQuery] int rofoId, [FromQuery] int verId)
    {
        if (rofoId <= 0 || verId <= 0)
        {
            return BadRequest(new { title = "Invalid Rofo ID or Version ID." });
        }

        var result = await _reportVersionService.GetAllFgCostVerDetailsByRofoIdAsync(rofoId, verId);
        if (result is not { IsSuccess: true, Data: not null })
        {
            return BadRequest(new { title = string.Join(", ", result.Errors) });
        }
        
        return Ok(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "costing")]
    public async Task<IActionResult> InsertVersion([FromBody] InsertVersionRequest request)
    {
        if (request.RoomId <= 0 || string.IsNullOrWhiteSpace(request.Notes))
        {
            return BadRequest(new { title = "Invalid input data." });
        }

        var result = await _reportVersionService.InsertVersionAsync(request.RoomId, request.Notes);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { title = string.Join(", ", result.Errors) });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetVersionRoomIds()
    {
        var result = await _reportVersionService.GetVersionRoomIdsAsync();
        if (result is not { IsSuccess: true, Data: not null })
        {
            return BadRequest(new { title = string.Join(", ", result.Errors) });
        }
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetVersions(int room)
    {
        if (room <= 0)
        {
            return BadRequest(new { title = "Invalid room ID." });
        }

        var result = await _reportVersionService.GetVersionsAsync(room);
        if (result is not { IsSuccess: true, Data: not null })
        {
            return BadRequest(new { title = string.Join(", ", result.Errors) });
        }
        
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllMupVer([FromQuery] int room, [FromQuery] int verId)
    {
        if (room <= 0)
        {
            return BadRequest(new { title = "Invalid room ID." });
        }
        
        if (verId <= 0)
        {
            return BadRequest(new { title = "Invalid version ID." });
        }

        var result = await _reportVersionService.GetAllMupVerAsync(room, verId);
        if (result is not { IsSuccess: true, Data: not null })
        {
            return BadRequest(new { title = string.Join(", ", result.Errors) });
        }
        
        return Ok(result);
    }
}