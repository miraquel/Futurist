using ClosedXML.Excel;
using Futurist.Common.Helpers;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

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
        
        return View(new MupSpDto());
    }

    public async Task<IActionResult> DownloadMupResult([FromQuery] int room)
    {
        var response = await _mupService.MupResultAsync(room);

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);

        var stream = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            row.Cell(1).Value = dto.Room;
            row.Cell(2).Value = dto.ProductId;
            row.Cell(3).Value = dto.ProductName;
            
            row.Cell(4).Value = dto.RofoDate;
            row.Cell(4).Style.NumberFormat.Format = "dd MMM yyyy";
            
            row.Cell(5).Value = dto.QtyRofo;
            row.Cell(5).Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.Integer;
            
            row.Cell(6).Value = dto.ItemId;
            row.Cell(7).Value = dto.ItemName;
            row.Cell(8).Value = dto.GroupSubstitusi;
            row.Cell(9).Value = dto.ItemAllocatedId;
            row.Cell(10).Value = dto.ItemAllocatedName;
            row.Cell(11).Value = dto.UnitId;
            row.Cell(12).Value = dto.InventBatch;
            
            row.Cell(13).Value = dto.Qty;
            row.Cell(13).Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.Precision2WithSeparatorAndParens;
            
            row.Cell(14).Value = dto.Price;
            row.Cell(14).Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.Integer;
            
            row.Cell(15).Value = dto.Source;
            row.Cell(16).Value = dto.RefId;
            
            row.Cell(17).Value = dto.LatestPurchasePrice;
            row.Cell(17).Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.Integer;
            
            row.Cell(18).Value = dto.Gap;
            row.Cell(18).Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.Integer;
        });
        
        // download as excel file
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MupResult_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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
        
        return BadRequest(response.Errors);
    }
    
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MupResultPagedList([FromQuery] PagedListRequestDto<MupSpDto> pagedListRequestDto)
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
    public IActionResult MupInProcessRoomIds()
    {
        return Ok(_mupService.MupInProcessRoomIds());
    }
}