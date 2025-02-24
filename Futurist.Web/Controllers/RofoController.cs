using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class RofoController : Controller
{
    private readonly IRofoService _rofoService;

    public RofoController(IRofoService rofoService)
    {
        _rofoService = rofoService;
    }

    // GET: RofoController with pagination
    public IActionResult Index()
    {
        if (TempData["Success"] != null)
        {
            ViewBag.Success = TempData["Success"];
        }
        if (TempData["Errors"] != null)
        {
            ViewBag.Errors = TempData["Errors"];
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
        
        var response = await _rofoService.ImportAsync(fileInput.OpenReadStream());
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
    public async Task<IActionResult> GetRofoPagedList([FromQuery] PagedListRequestDto<RofoDto> pagedListRequest)
    {
        var response = await _rofoService.GetPagedListAsync(pagedListRequest);
        return Ok(response);
    }
}