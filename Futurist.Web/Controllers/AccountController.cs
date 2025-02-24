using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurist.Web.Controllers;

public class AccountController : Controller
{
    [Authorize(Roles = "manage-account")]
    public IActionResult Index()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        ViewBag.Claims = claims;
        return View();
    }
    
    public IActionResult Logout()
    {
        var callbackUrl = Url.Action("LoggedOut", "Account", values: null, protocol: Request.Scheme);
        return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl },
                       CookieAuthenticationDefaults.AuthenticationScheme, 
                       OpenIdConnectDefaults.AuthenticationScheme);
    }

    public IActionResult LoggedOut()
    {
        return View();
    }
    
    // Access denied
    public IActionResult AccessDenied()
    {
        // Retrieve the previous page URL from the request headers.
        var previousUrl = Request.Headers["Referer"].ToString();
        var returnUrl = Request.Query["ReturnUrl"].ToString();
        // Fallback if the Referer header is empty.
        if (string.IsNullOrEmpty(previousUrl))
        {
            previousUrl = "/";
        }
        // Fallback if the ReturnUrl query string is empty.
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "/";
        }
        ViewBag.PreviousUrl = previousUrl;
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
}