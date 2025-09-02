using DocumentFormat.OpenXml.Bibliography;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

[Authorize(Roles = "costing,sc,rni,finance,admin,marketing_domestik,marketing_export")]
[Route("[controller]/[action]")]
public class AnlRmController : Controller
{
    public IActionResult AnlRmPrice()
    {
        return View();
    }
    
    public IActionResult AnlKurs()
    {
        return View();
    }
    
    public IActionResult AnlFgPrice()
    {
        return View();
    }
    
    public IActionResult AnlPmPrice()
    {
        return View();
    }
    
    public IActionResult AnlRmPriceGroup()
    {
        return View();
    }
}

[ApiController]
[Authorize(Roles = "costing,sc,rni,finance,admin,marketing_domestik,marketing_export")]
[Route("api/[controller]/[action]")]
public class AnlRmApiController : ControllerBase
{
    private readonly IAnlRmService _anlRmService;

    public AnlRmApiController(IAnlRmService anlRmService)
    {
        _anlRmService = anlRmService;
    }
    
    // Get Room IDs
    [HttpGet]
    public async Task<IActionResult> GetRoomIdsAsync(CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetRoomIdsAsync(cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
    
    // Get Version IDs
    [HttpGet]
    public async Task<IActionResult> GetVerIdsAsync([FromQuery] int room, CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetRofoVerIdsAsync(room, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAnlFgPriceAsync([FromQuery] int room, [FromQuery] int verId, [FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetAnlFgPriceAsync(room, verId, year, month, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAnlRmPriceGroupAsync([FromQuery] int room, [FromQuery] int verId, [FromQuery] int year, [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetAnlRmPriceGroupAsync(room, verId, year, month, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
    
    // Get Years
    [HttpGet]
    public async Task<IActionResult> GetYearsAsync([FromQuery] int room, [FromQuery] int verId, CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetYearsAsync(room, verId, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
   
    // Get Months
    [HttpGet]
    public async Task<IActionResult> GetMonthsAsync([FromQuery] int room, [FromQuery] int verId, [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetMonthsAsync(room, verId, year, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAnlRmpPrice([FromQuery] int room, [FromQuery] int verId, [FromQuery] int year, [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetAnlRmpPrice(room, verId, year, month, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAnlKursAsync([FromQuery] int version, CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetAnlKursAsync(version, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAnlPmPriceAsync([FromQuery] int room, [FromQuery] int verId, [FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var response = await _anlRmService.GetAnlPmPriceAsync(room, verId, year, month, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
}