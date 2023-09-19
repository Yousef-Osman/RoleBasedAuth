using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedAuth.Data;

namespace RoleBasedAuth.Controllers;
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var model = _roleManager.Roles.ToList();
        return View(model);
    }

    public IActionResult Roles()
    {
        var model = _roleManager.Roles.ToList();
        return View(model);
    }

    public IActionResult AddRole()
    {
        var model = new IdentityRole();
        return View(model);
    }

    [HttpPost]
    public IActionResult AddRole(IdentityRole model)
    {

        return View(model);
    }
}

