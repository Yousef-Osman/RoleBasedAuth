using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoleBasedAuth.Data;

namespace RoleBasedAuth.Controllers;
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }

    public IActionResult AddRole()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(IdentityRole model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _roleManager.RoleExistsAsync(model.Name))
        {
            ModelState.AddModelError("Name", "Role exists");
            return View(model);
        }

        await _roleManager.CreateAsync(new IdentityRole(model.Name));

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EditRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return BadRequest();

        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(IdentityRole model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var role = await _roleManager.FindByIdAsync(model.Id);

        if (role == null)
        {
            ModelState.AddModelError("Name", "no such Role exists");
            return View(model);
        }

        role.Name = model.Name;
        role.NormalizedName = model.Name.ToUpper();
        await _roleManager.UpdateAsync(role);

        return RedirectToAction(nameof(Index));
    }
}
