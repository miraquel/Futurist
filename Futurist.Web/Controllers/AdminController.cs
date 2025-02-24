using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class AdminController : Controller
{
    // GET
    [Authorize(Roles = "admin")]
    public IActionResult Index()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        ViewBag.Claims = claims;
        return View();
    }
}