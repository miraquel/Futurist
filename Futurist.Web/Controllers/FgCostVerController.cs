using Futurist.Common.Helpers;
using Futurist.Repository.Command.FgCostVerCommand;
using Futurist.Service.Interface;
using Futurist.Web.Requests.FgCostVer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
public class FgCostVerController : Controller
{
    private readonly IFgCostVerService _fgCostVerService;

    public FgCostVerController(IFgCostVerService fgCostVerService)
    {
        _fgCostVerService = fgCostVerService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var response = await _fgCostVerService.GetFgCostVerRoomIdsAsync();
        
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

    [HttpGet]
    public async Task<IActionResult> Process()
    {
        var response = await _fgCostVerService.GetFgCostVerRoomIdsAsync();
        
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
    
    public async Task<IActionResult> DownloadSummaryFgCostVer([FromQuery] int room)
    {
        var response = await _fgCostVerService.GetAllFgCostVerAsync(room);
    
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
                row.Cell(2).Value = dto.ItemId;
                row.Cell(3).Value = dto.ItemName;
                row.Cell(4).Value = dto.Unit;
                row.Cell(5).Value = dto.InKg;
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
[Authorize]
[Route("api/[controller]/[action]")]
public class FgCostVerApiController : ControllerBase
{
    private readonly IFgCostVerService _fgCostVerService;

    public FgCostVerApiController(IFgCostVerService fgCostVerService)
    {
        _fgCostVerService = fgCostVerService;
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetAllFgCostVer(int roomId)
    {
        var result = await _fgCostVerService.GetAllFgCostVerAsync(roomId);
        return Ok(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InsertFgCostVer([FromBody] InsertFgCostVerRequest request)
    {
        if (request.RoomId <= 0 || string.IsNullOrWhiteSpace(request.Notes))
        {
            return BadRequest(new { title = "Invalid input data." });
        }

        var result = await _fgCostVerService.InsertFgCostVerAsync(request.RoomId, request.Notes);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { title = string.Join(", ", result.Errors) });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFgCostVerRoomIds()
    {
        var result = await _fgCostVerService.GetFgCostVerRoomIdsAsync();
        return Ok(result);
    }
}