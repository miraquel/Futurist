using ClosedXML.Excel;
using Futurist.Common.Helpers;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class FgCostController : Controller
{
    private readonly IFgCostService _fgCostService;

    public FgCostController(IFgCostService fgCostService)
    {
        _fgCostService = fgCostService;
    }
    
    public async Task<IActionResult> Index()
    {
        var response = await _fgCostService.GetFgCostRoomIdsAsync();
        
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
    
    public async Task<IActionResult> Details()
    {
        var response = await _fgCostService.GetFgCostRoomIdsAsync();
        
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
    
    public async Task<IActionResult> Process()
    {
        var response = await _fgCostService.GetFgCostRoomIdsAsync();
        
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
    
    public IActionResult DetailsByRofoId(int id)
    {
        ViewBag.RofoId = id;
        return View();
    }
    
    public async Task<IActionResult> DownloadSummaryFgCost([FromQuery] int room)
    {
        var response = await _fgCostService.GetSummaryFgCostAsync(room);

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);
        
        var result = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Rofo ID";
                row.Cell(3).Value = "Product ID";
                row.Cell(4).Value = "Product Name";
                row.Cell(5).Value = "Unit";
                row.Cell(6).Value = "Unit in Kg";
                row.Cell(7).Value = "Rofo Date";
                row.Cell(8).Value = "Rofo Qty";
                row.Cell(9).Value = "Yield";
                row.Cell(10).Value = "RM Price";
                row.Cell(11).Value = "PM Price";
                row.Cell(12).Value = "Std Cost Price";
                row.Cell(13).Value = "Cost Price";
                row.Cell(14).Value = "Sales Price Index";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.RofoId;
                row.Cell(3).Value = dto.ProductId;
                row.Cell(4).Value = dto.ProductName;
                row.Cell(5).Value = dto.Unit;
                row.Cell(6).Value = dto.UnitInKg;
                row.Cell(7).Value = dto.RofoDate;
                row.Cell(7).Style.NumberFormat.Format = "dd MMM yyyy";
                row.Cell(8).Value = dto.RofoQty;
                row.Cell(8).Style.NumberFormat.Format = "#,##0";
                row.Cell(9).Value = dto.Yield;
                row.Cell(9).Style.NumberFormat.Format = "#,##0.0%";
                row.Cell(10).Value = dto.RmPrice;
                row.Cell(10).Style.NumberFormat.Format = "#,##0";
                row.Cell(11).Value = dto.PmPrice;
                row.Cell(11).Style.NumberFormat.Format = "#,##0";
                row.Cell(12).Value = dto.StdCostPrice;
                row.Cell(12).Style.NumberFormat.Format = "#,##0";
                row.Cell(13).Value = dto.CostPrice;
                row.Cell(13).Style.NumberFormat.Format = "#,##0";
                row.Cell(14).Value = dto.SalesPriceIndex;
                row.Cell(14).Style.NumberFormat.Format = "#,##0";
                row.Cell(14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }
        });
        
        // download as excel file
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SummaryFgCost_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    public async Task<IActionResult> DownloadFgCostDetail([FromQuery] int room)
    {
        var response = await _fgCostService.GetFgCostDetailAsync(room);

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);

        var result = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Rofo ID";
                row.Cell(3).Value = "Product ID";
                row.Cell(4).Value = "Product Name";
                row.Cell(5).Value = "Rofo Date";
                row.Cell(6).Value = "Rofo Qty";
                row.Cell(7).Value = "Item ID";
                row.Cell(8).Value = "Item Name";
                row.Cell(9).Value = "Group Substitusi";
                row.Cell(10).Value = "Item Allocated ID";
                row.Cell(11).Value = "Item Allocated Name";
                row.Cell(12).Value = "Batch";
                row.Cell(13).Value = "Qty";
                row.Cell(14).Value = "Price";
                row.Cell(15).Value = "RM Price";
                row.Cell(16).Value = "PM Price";
                row.Cell(17).Value = "Std Cost Price";
                row.Cell(18).Value = "Source";
                row.Cell(19).Value = "Ref ID";
                row.Cell(20).Value = "Latest Purchase Price";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.RofoId;
                row.Cell(3).Value = dto.ProductId;
                row.Cell(4).Value = dto.ProductName;
                row.Cell(5).Value = dto.RofoDate;
                row.Cell(5).Style.NumberFormat.Format = "dd MMM yyyy";
                row.Cell(6).Value = dto.RofoQty;
                row.Cell(7).Value = dto.ItemId;
                row.Cell(8).Value = dto.ItemName;
                row.Cell(9).Value = dto.GroupSubstitusi;
                row.Cell(10).Value = dto.ItemAllocatedId;
                row.Cell(11).Value = dto.ItemAllocatedName;
                row.Cell(12).Value = dto.InventBatch;
                row.Cell(13).Value = dto.Qty;
                row.Cell(14).Value = dto.Price;
                row.Cell(15).Value = dto.RmPrice;
                row.Cell(16).Value = dto.PmPrice;
                row.Cell(17).Value = dto.StdCostPrice;
                row.Cell(18).Value = dto.Source;
                row.Cell(19).Value = dto.RefId;
                row.Cell(20).Value = dto.LatestPurchasePrice;
            }
        });

        // download as excel file
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"FgCostDetail_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    public async Task<IActionResult> DownloadFgCostDetailsByRofoId([FromQuery] int rofoId)
    {
        var response = await _fgCostService.GetFgCostDetailsByRofoIdFromSpAsync(rofoId);

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);

        var result = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Rofo ID";
                row.Cell(3).Value = "Product ID";
                row.Cell(4).Value = "Product Name";
                row.Cell(5).Value = "Rofo Date";
                row.Cell(6).Value = "Rofo Qty";
                row.Cell(7).Value = "Item ID";
                row.Cell(8).Value = "Item Name";
                row.Cell(9).Value = "Group Substitusi";
                row.Cell(10).Value = "Item Allocated ID";
                row.Cell(11).Value = "Item Allocated Name";
                row.Cell(12).Value = "Batch";
                row.Cell(13).Value = "Qty";
                row.Cell(14).Value = "Price";
                row.Cell(15).Value = "RM Price";
                row.Cell(16).Value = "PM Price";
                row.Cell(17).Value = "Std Cost Price";
                row.Cell(18).Value = "Source";
                row.Cell(19).Value = "Ref ID";
                row.Cell(20).Value = "Latest Purchase Price";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.RofoId;
                row.Cell(3).Value = dto.ProductId;
                row.Cell(4).Value = dto.ProductName;
                row.Cell(5).Value = dto.RofoDate;
                row.Cell(5).Style.NumberFormat.Format = "dd MMM yyyy";
                row.Cell(6).Value = dto.RofoQty;
                row.Cell(7).Value = dto.ItemId;
                row.Cell(8).Value = dto.ItemName;
                row.Cell(9).Value = dto.GroupSubstitusi;
                row.Cell(10).Value = dto.ItemAllocatedId;
                row.Cell(11).Value = dto.ItemAllocatedName;
                row.Cell(12).Value = dto.InventBatch;
                row.Cell(13).Value = dto.Qty;
                row.Cell(13).Style.NumberFormat.Format = "#,##0.00";
                row.Cell(14).Value = dto.Price;
                row.Cell(14).Style.NumberFormat.Format = "#,##0";
                row.Cell(15).Value = dto.RmPrice;
                row.Cell(15).Style.NumberFormat.Format = "#,##0";
                row.Cell(16).Value = dto.PmPrice;
                row.Cell(16).Style.NumberFormat.Format = "#,##0";
                row.Cell(17).Value = dto.StdCostPrice;
                row.Cell(17).Style.NumberFormat.Format = "#,##0";
                row.Cell(18).Value = dto.Source;
                row.Cell(19).Value = dto.RefId;
                row.Cell(20).Value = dto.LatestPurchasePrice;
                row.Cell(20).Style.NumberFormat.Format = "#,##0";
            }
        });
        
        // download as excel file
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"FgCostDetailsByRofoId_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class FgCostApiController : ControllerBase
{
    private readonly IFgCostService _fgCostService;

    public FgCostApiController(IFgCostService fgCostService)
    {
        _fgCostService = fgCostService;
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetSummaryFgCostAsync([FromQuery] int roomId)
    {
        var response = await _fgCostService.GetSummaryFgCostAsync(roomId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetSummaryFgCostPagedListAsync([FromQuery] PagedListRequestDto dto)
    {
        var response = await _fgCostService.GetSummaryFgCostPagedListAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetFgCostDetailAsync([FromQuery] int roomId)
    {
        var response = await _fgCostService.GetFgCostDetailAsync(roomId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetFgCostDetailPagedListAsync([FromQuery] PagedListRequestDto dto)
    {
        var response = await _fgCostService.GetFgCostDetailPagedListAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetFgCostDetailsByRofoIdFromSpAsync([FromQuery] int rofoId)
    {
        var response = await _fgCostService.GetFgCostDetailsByRofoIdFromSpAsync(rofoId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetFgCostDetailsByRofoIdPagedListAsync([FromQuery] PagedListRequestDto dto)
    {
        var response = await _fgCostService.GetFgCostDetailsByRofoIdPagedListAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CalculateFgCost([FromBody] int roomId)
    {
        return Ok(_fgCostService.CalculateFgCostJob(roomId));
    }

    [HttpGet]
    public IActionResult GetInProcessRoomIds()
    {
        var inProcessRoomIds = _fgCostService.GetFgCostInProcessRoomIds();
        return Ok(inProcessRoomIds);
    }
}