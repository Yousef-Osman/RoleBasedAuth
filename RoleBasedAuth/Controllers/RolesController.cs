﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RoleBasedAuth.Data;
using RoleBasedAuth.Models.Enums;
using RoleBasedAuth.ViewModels;
using System.Data;

namespace RoleBasedAuth.Controllers;

[Authorize(Roles = nameof(UserRoles.SuperAdmin))]
public class RolesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(RoleVM model)
    {
        if (!ModelState.IsValid)
            return Json(new JsonResult { isSuccess = false, errors = GetErrorList(ModelState) });

        if (await _roleManager.RoleExistsAsync(model.Name))
        {
            ModelState.AddModelError("Name", "Role exists");
            return Json(new JsonResult { isSuccess = false, errors = GetErrorList(ModelState) });
        }

        await _roleManager.CreateAsync(new IdentityRole(model.Name));

        return Ok(new JsonResult { isSuccess = true });
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

    [HttpDelete]
    public async Task<IActionResult> DeleteRole([FromBody] RoleVM model)
    {
        if (string.IsNullOrWhiteSpace(model?.Id))
            return Json(new JsonResult { isSuccess = false, errorMessage = "Invalid role" });

        var role = await _roleManager.FindByIdAsync(model.Id);
        if (role == null)
            return Json(new JsonResult { isSuccess = false, errorMessage = "Role doesn't exists." });

        var exists = _context.UserRoles.Any(a => a.RoleId == role.Id);
        if (exists)
            return Json(new JsonResult { isSuccess = false, errorMessage = "Can't delete role." });

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
            return Ok(new JsonResult { isSuccess = true });
        else
            return Json(new { isSuccess = false, errorMessage = "Something went wrong." });
    }

    private List<string> GetErrorList(ModelStateDictionary modelState)
    {
        return modelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
    }
}

public class JsonResult
{
    public bool isSuccess { get; set; }
    public List<string> errors { get; set; }
    public string errorMessage { get; set; }
}
