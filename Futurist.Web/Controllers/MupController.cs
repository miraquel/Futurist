using Futurist.Common.Helpers;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class MupController : Controller
{
    private readonly IMupService _mupService;

    public MupController(IMupService mupService)
    {
        _mupService = mupService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        
        var response = await _mupService.GetMupRoomIdsAsync();
        
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
    
    public async Task<IActionResult> SummaryByItemId()
    {
        var response = await _mupService.GetMupRoomIdsAsync();
        
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
    
    public async Task<IActionResult> SummaryByBatchNumber()
    {
        var response = await _mupService.GetMupRoomIdsAsync();
        
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
    
    public async Task<IActionResult> Process()
    {
        var response = await _mupService.GetMupRoomIdsAsync();
        
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

    public async Task<IActionResult> Details()
    {
        var response = await _mupService.GetMupRoomIdsAsync();
        
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

    public async Task<IActionResult> DownloadMupResult([FromQuery] int room)
    {
        var response = await _mupService.MupResultAsync(room);

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);

        var stream = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Product Id";
                row.Cell(3).Value = "Product Name";
                row.Cell(4).Value = "Rofo Date";
                row.Cell(5).Value = "Rofo Qty";
                row.Cell(6).Value = "Item Id";
                row.Cell(7).Value = "Item Name";
                row.Cell(8).Value = "Group Substitusi";
                row.Cell(9).Value = "Group Procurement";
                row.Cell(10).Value = "Item Allocated Id";
                row.Cell(11).Value = "Item Allocated Name";
                row.Cell(12).Value = "Unit";
                row.Cell(13).Value = "Batch";
                row.Cell(14).Value = "Qty";
                row.Cell(15).Value = "Price";
                row.Cell(16).Value = "Source";
                row.Cell(17).Value = "Original Currency";
                row.Cell(18).Value = "Ref Id";
                row.Cell(19).Value = "Latest Purchase Price";
                row.Cell(20).Value = "Gap";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.ProductId;
                row.Cell(3).Value = dto.ProductName;
                row.Cell(4).Value = dto.RofoDate;
                row.Cell(4).Style.NumberFormat.Format = "dd MMM yyyy";
                row.Cell(5).Value = dto.QtyRofo;
                row.Cell(5).Style.NumberFormat.Format = "#,##0";
                row.Cell(6).Value = dto.ItemId;
                row.Cell(7).Value = dto.ItemName;
                row.Cell(8).Value = dto.GroupSubstitusi;
                row.Cell(9).Value = dto.GroupProcurement;
                row.Cell(10).Value = dto.ItemAllocatedId;
                row.Cell(11).Value = dto.ItemAllocatedName;
                row.Cell(12).Value = dto.UnitId;
                row.Cell(13).Value = dto.InventBatch;
                row.Cell(14).Value = dto.Qty;
                row.Cell(14).Style.NumberFormat.Format = "#,##0.00";
                row.Cell(15).Value = dto.Price;
                row.Cell(15).Style.NumberFormat.Format = "#,##0";
                row.Cell(16).Value = dto.Source;
                row.Cell(17).Value = dto.OriginalCurrency;
                row.Cell(18).Value = dto.RefId;
                row.Cell(19).Value = dto.LatestPurchasePrice;
                row.Cell(19).Style.NumberFormat.Format = "#,##0";
                row.Cell(20).Value = dto.Gap;
                row.Cell(20).Style.NumberFormat.Format = "#,##0.0%";
            }
        });
        
        // download as excel file
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MupResult_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
    
    public async Task<IActionResult> DownloadMupSummaryByItemId([FromQuery] int room)
    {
        var response = await _mupService.MupSummaryByItemIdFromSpAsync(room);

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);
        
        var result = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Mup Date";
                row.Cell(3).Value = "Group Substitusi";
                row.Cell(4).Value = "Group Procurement"; // added header
                row.Cell(5).Value = "Item Id";
                row.Cell(6).Value = "Item Name";
                row.Cell(7).Value = "Qty";
                row.Cell(8).Value = "Price";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.MupDate;
                row.Cell(2).Style.NumberFormat.Format = "dd MMM yyyy";
                row.Cell(3).Value = dto.GroupSubstitusi;
                row.Cell(4).Value = dto.GroupProcurement; // added mapping
                row.Cell(5).Value = dto.ItemId;
                row.Cell(6).Value = dto.ItemName;
                row.Cell(7).Value = dto.Qty;
                row.Cell(7).Style.NumberFormat.Format = "#,##0.00";
                row.Cell(8).Value = dto.Price;
                row.Cell(8).Style.NumberFormat.Format = "#,##0";
            }
        });
        
        // download as excel file
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MupSummaryByItemId_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    public async Task<IActionResult> DownloadMupSummaryByBatchNumber([FromQuery] int room)
    {
        var response = await _mupService.MupSummaryByBatchNumberFromSpAsync(room);
        
        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);
        
        var stream = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "MUP Date";
                row.Cell(3).Value = "Source";
                row.Cell(4).Value = "Group Substitusi";
                row.Cell(5).Value = "Group Procurement"; // added header
                row.Cell(6).Value = "Item Allocated ID";
                row.Cell(7).Value = "Item Allocated Name";
                row.Cell(8).Value = "Unit";
                row.Cell(9).Value = "Batch";
                row.Cell(10).Value = "Qty";
                row.Cell(11).Value = "Price";
                row.Cell(12).Value = "Latest Purchase Price";
                row.Cell(13).Value = "Gap";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.MupDate;
                row.Cell(2).Style.DateFormat.Format = "dd MMM yyyy";
                row.Cell(3).Value = dto.Source;
                row.Cell(4).Value = dto.GroupSubstitusi;
                row.Cell(5).Value = dto.GroupProcurement; // added mapping
                row.Cell(6).Value = dto.ItemAllocatedId;
                row.Cell(7).Value = dto.ItemAllocatedName;
                row.Cell(8).Value = dto.UnitId;
                row.Cell(9).Value = dto.InventBatch;
                row.Cell(10).Value = dto.Qty;
                row.Cell(10).Style.NumberFormat.Format = "#,##0.00";
                row.Cell(11).Value = dto.Price;
                row.Cell(11).Style.NumberFormat.Format = "#,##0";
                row.Cell(12).Value = dto.LatestPurchasePrice;
                row.Cell(12).Style.NumberFormat.Format = "#,##0";
                row.Cell(13).Value = dto.Gap;
                row.Cell(13).Style.NumberFormat.Format = "#,##0.0%";
            }
        });
        
        // download as excel file
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MupSummaryByBatchNumber_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class MupApiController : ControllerBase
{
    private readonly IMupService _mupService;

    public MupApiController(IMupService mupService)
    {
        _mupService = mupService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessMup([FromBody] int roomId)
    {
        return Ok(_mupService.ProcessMupJob(roomId));
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupResult([FromQuery] int roomId)
    {
        var response = await _mupService.MupResultAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupResultPagedList([FromQuery] PagedListRequestDto pagedListRequestDto)
    {
        var response = await _mupService.MupResultPagedListAsync(pagedListRequestDto);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupSummaryByItemIdFromSp([FromQuery] int roomId)
    {
        var response = await _mupService.MupSummaryByItemIdFromSpAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupSummaryByItemId([FromQuery] ListRequestDto listRequestDto)
    {
        var response = await _mupService.MupSummaryByItemIdAsync(listRequestDto);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response.Errors);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupSummaryByItemIdPagedList([FromQuery] PagedListRequestDto pagedListRequestDto)
    {
        var response = await _mupService.MupSummaryByItemIdPagedListAsync(pagedListRequestDto);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupSummaryByBatchNumberFromSp([FromQuery] int roomId)
    {
        var response = await _mupService.MupSummaryByBatchNumberFromSpAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupSummaryByBatchNumber([FromQuery] ListRequestDto listRequestDto)
    {
        var response = await _mupService.MupSummaryByBatchNumberAsync(listRequestDto);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupSummaryByBatchNumberPagedList([FromQuery] PagedListRequestDto pagedListRequestDto)
    {
        var response = await _mupService.MupSummaryByBatchNumberPagedListAsync(pagedListRequestDto);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public IActionResult MupInProcessRoomIds()
    {
        return Ok(_mupService.MupInProcessRoomIds());
    }
}