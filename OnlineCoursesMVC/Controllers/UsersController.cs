using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    // Внедряем UserManager и RoleManager
    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: /Users (список)
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    // GET: Users/Delete/GUID
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    // POST: Users/Delete/GUID
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            // Не позволяем удалить самого себя
            if (_userManager.GetUserId(User) == id)
            {
                // Можно добавить сообщение об ошибке
                return RedirectToAction(nameof(Index));
            }
            var result = await _userManager.DeleteAsync(user);
            // Здесь можно добавить проверку result.Succeeded
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Users/ManageRoles/GUID
    public async Task<IActionResult> ManageRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var viewModel = new ManageUserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = new List<RoleViewModel>()
        };

        var allRoles = await _roleManager.Roles.ToListAsync();
        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in allRoles)
        {
            viewModel.Roles.Add(new RoleViewModel
            {
                RoleName = role.Name,
                IsSelected = userRoles.Contains(role.Name)
            });
        }

        return View(viewModel);
    }

    // POST: Users/ManageRoles/GUID
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        // Удаляем все текущие роли
        await _userManager.RemoveFromRolesAsync(user, userRoles);
        // Добавляем только выбранные
        await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));

        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // По умолчанию назначаем роль "User"
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }
}