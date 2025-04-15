using ClosedXML.Excel;
using Futurist.Common.Helpers;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class BomStdController : Controller
{
    private readonly IBomStdService _bomStdService;

    public BomStdController(IBomStdService bomStdService)
    {
        _bomStdService = bomStdService;
    }
    
    public async Task<IActionResult> Index()
    {
        var response = await _bomStdService.GetBomStdRoomIdsAsync();
        
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

    public async Task<IActionResult> DownloadBomErrorCheck([FromQuery] int room)
    {
        var response = await _bomStdService.BomErrorCheckAsync(room);

        if (response is not { IsSuccess: true, Data: not null }) return BadRequest(response.Errors);
        
        var stream = ExcelHelper.ExportExcel(response.Data, (row, dto) =>
        {
            if (row.RowNumber() == 1)
            {
                row.Cell(1).Value = "Room";
                row.Cell(2).Value = "Product Id";
                row.Cell(3).Value = "Product Name";
                row.Cell(4).Value = "Bom Id";
                row.Cell(5).Value = "Item Id";
                row.Cell(6).Value = "Item Name";
            }
            else
            {
                row.Cell(1).Value = dto.Room;
                row.Cell(2).Value = dto.ProductId;
                row.Cell(3).Value = dto.ProductName;
                row.Cell(4).Value = dto.BomId;
                row.Cell(5).Value = dto.ItemId;
                row.Cell(6).Value = dto.ItemName;
            }
        });
        
        return File(stream, "text/csv", $"BomErrorCheck_{DateTime.Now:yyyyMMddHHmmss}.csv");
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class BomStdApiController : ControllerBase
{
    private readonly IBomStdService _bomStdService;

    public BomStdApiController(IBomStdService bomStdService)
    {
        _bomStdService = bomStdService;
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BomErrorCheck([FromQuery] int roomId)
    {
        var response = await _bomStdService.BomErrorCheckAsync(roomId);
        
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        
        return BadRequest(response.Errors);
    }
    
    [HttpGet]
    public async Task<IActionResult> BomErrorCheckPagedList([FromQuery] PagedListRequestDto pagedListRequestDto)
    {
        var response = await _bomStdService.BomErrorCheckPagedListAsync(pagedListRequestDto);
        
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessBomStd([FromBody] int roomId)
    {
        return Ok(_bomStdService.ProcessBomStdJob(roomId));
    }

    [HttpGet]
    public IActionResult GetInProcessRoomIds()
    {
        var inProcessRoomIds = _bomStdService.GetBomStdInProcessRoomIds();
        return Ok(inProcessRoomIds);
    }
}