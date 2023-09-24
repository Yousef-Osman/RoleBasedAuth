using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoleBasedAuth.Data;
using RoleBasedAuth.Models.Enums;
using RoleBasedAuth.ViewModels;
using System.Data;

namespace RoleBasedAuth.Controllers;

[Authorize(Roles = nameof(UserRoles.SuperAdmin) + "," + nameof(UserRoles.Admin))]
public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.Select(a => new ApplicationUser
        {
            Id = a.Id,
            UserName = a.UserName
        }).ToListAsync();

        return View(users);
    }

    public async Task<IActionResult> EditUserRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return BadRequest();

        var roles = await _roleManager.Roles.ToListAsync();

        var viewModel = new UserRolesVM()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = roles.Select(role => new RoleVM
            {
                Id = role.Id,
                Name = role.Name,
                IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUserRole(UserRolesVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
            return BadRequest();

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in model.Roles)
        {
            if (userRoles.Any(a => a == role.Name) && !role.IsSelected)
                await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (!userRoles.Any(a => a == role.Name) && role.IsSelected)
                await _userManager.AddToRoleAsync(user, role.Name);
        }

        return RedirectToAction(nameof(Users));
    }
}

