using System.Diagnostics;
using Futurist.Service;
using Microsoft.AspNetCore.Mvc;
using Futurist.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Futurist.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger _logger = Log.ForContext<HomeController>();
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature is { Error: ServiceException })
        {
            _logger.Error(exceptionHandlerPathFeature.Error, "Service exception occurred");
        }
        else if (exceptionHandlerPathFeature is not null)
        {
            _logger.Error(exceptionHandlerPathFeature.Error, "An error occurred");
        }

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}