using Futurist.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class CurrentDataController : Controller
{
    public IActionResult ItemOnHand()
    {
        return View();
    }
    
    public IActionResult ItemPoIntransit()
    {
        return View();
    }
    
    public IActionResult ItemPag()
    {
        return View();
    }
}

[Route("api/[controller]/[action]")]
[ApiController]
public class CurrentDataApiController : ControllerBase
{
    private readonly ICurrentDataService _currentDataService;

    public CurrentDataApiController(ICurrentDataService currentDataService)
    {
        _currentDataService = currentDataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetItemOnHandAsync(CancellationToken cancellationToken)
    {
        var response = await _currentDataService.GetItemOnHandAsync(cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetItemPoIntransitAsync(CancellationToken cancellationToken)
    {
        var response = await _currentDataService.GetItemPoIntransitAsync(cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetItemPagAsync(CancellationToken cancellationToken)
    {
        var response = await _currentDataService.GetItemPagAsync(cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
}