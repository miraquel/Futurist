using Futurist.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class RmForecastController : Controller
{
    public IActionResult RmForecast()
    {
        return View();
    }
}

[Route("api/[controller]")]
[ApiController]
public class RmForecastApiController : ControllerBase
{
    private readonly IRmForecastService _rmForecastService;

    public RmForecastApiController(IRmForecastService rmForecastService)
    {
        _rmForecastService = rmForecastService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllAsync([FromQuery] int room, [FromQuery] int year, CancellationToken cancellationToken)
    {
        var response = await _rmForecastService.GetAllAsync(room, year, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }

    [HttpGet("GetYears")]
    public async Task<IActionResult> GetYearsAsync([FromQuery] int room, CancellationToken cancellationToken)
    {
        var response = await _rmForecastService.GetYearsAsync(room, cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }

    [HttpGet("GetRoomIds")]
    public async Task<IActionResult> GetRoomIdsAsync(CancellationToken cancellationToken)
    {
        var response = await _rmForecastService.GetRoomIdsAsync(cancellationToken);
        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
}