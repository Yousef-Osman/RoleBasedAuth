using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleBasedAuth.Models;
using RoleBasedAuth.Models.Enums;
using System.Data;
using System.Diagnostics;

namespace RoleBasedAuth.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin) + "," + nameof(UserRoles.Admin) + "," + nameof(UserRoles.Manager))]
    public IActionResult Manager()
    {
        return View();
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin) + "," + nameof(UserRoles.Admin))]
    public IActionResult Admin()
    {
        return View();
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    public IActionResult SuperAdmin()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
