﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;

namespace TaskManager.Controllers;

public class UserController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _signInManager = signInManager;
        _webHostEnvironment = webHostEnvironment;
    }


    [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
    public IActionResult Index(string sortOrder, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["CreatedAtSortParam"] = sortOrder == "createdAt" ? "createdAt_desc" : "createdAt";
        ViewData["LastNameSortParam"] = sortOrder == "lastName" ? "lastName_desc" : "lastName";
        ViewData["JobTitleSortParam"] = sortOrder == "jobTitle" ? "jobTitle_desc" : "jobTitle";

        var userList = _unitOfWork.UserRepository.GetUsers().ToList();
        var users = from u in userList select u;

        switch (sortOrder)
        {
            case "lastName_desc":
                users = users.OrderByDescending(p => p.LastName);
                break;
            case "createdAt":
                users = users.OrderBy(u => u.CreatedAt);
                break;
            case "createdAt_desc":
                users = users.OrderByDescending(u => u.CreatedAt);
                break;
            case "jobTitle":
                users = users.OrderBy(u => u.JobTitle);
                break;
            case "jobTitle_desc":
                users = users.OrderBy(u => u.JobTitle);
                break;
            default:
                users = users.OrderBy(p => p.LastName);
                break;
        }

        var uList = users.ToList();
        int pageSize = 10;
        return View(PaginatedList<ApplicationUser>.Create(uList, pageNumber ?? 1, pageSize));
    }


    public async Task<IActionResult> Details(string id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var user = await _unitOfWork.UserRepository.GetUserWithProjectsAndTickets(id);
        if (user is null)
        {
            return NotFound();
        }

        var open = user.Tickets.Where(x => x.Ticket!.Status != Core.Enums.Enums.Status.COMPLETED);
        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
        var vm = new UserViewModel
        {
            User = user,
            Roles = (List<string>)userRoles,
            ProjectAssignments = user.Projects,
            TicketsAssignments = user.Tickets
        };

        return View(vm);
    }


    [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
    public async Task<IActionResult> Edit(string id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var user = await _unitOfWork.UserRepository.GetUserAsync(id);
        var roles = _unitOfWork.RoleRepository.GetRoles();
        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
        var roleItems = new List<SelectListItem>();

        var roleItemsSelect = roles.Select(role =>
            new SelectListItem(
                role.Name,
                role.Id,
                userRoles.Any(r => r.Contains(role.Name)))).ToList();

        var vm = new EditUserViewModel
        {
            User = user,
            Roles = roleItemsSelect
        };

        return View(vm);
    }

    [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
    [HttpPost]
    public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
    {
        var user = await _unitOfWork.UserRepository.GetUserAsync(data.User.Id);
        if (user is null)
        {
            return NotFound();
        }
        var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);
        var rolesToAdd = new List<string>();
        var rolesToDelete = new List<string>();

        foreach (var role in data.Roles)
        {
            var assignedInDb = userRolesInDb.FirstOrDefault(r => r == role.Text);
            if (role.Selected)
            {
                if (assignedInDb is null)
                {
                    rolesToAdd.Add(role.Text);
                }
            }
            else
            {
                if (assignedInDb is not null)
                {
                    rolesToDelete.Add(role.Text);
                }
            }
        }
        if (rolesToAdd.Any())
        {
            await _signInManager.UserManager.AddToRolesAsync(user, rolesToAdd);
        }
        if (rolesToDelete.Any())
        {
            await _signInManager.UserManager.RemoveFromRolesAsync(user, rolesToDelete);
        }
        user.FirstName = data.User.FirstName;
        user.LastName = data.User.LastName;
        user.UserName = data.User.UserName;
        user.Email = data.User.Email;
        user.JobTitle = data.User.JobTitle;
        user.EmployeeID = data.User.EmployeeID;
        user.Email = data.User.Email;
        _unitOfWork.UserRepository.UpdateUser(user);
        await _unitOfWork.SaveAsync();
        return RedirectToAction("Details", new { id = user.Id });
    }



    [AllowAnonymous]
    public async Task<IActionResult> Demo(string userRole)
    {
        string demoEmail = $"{userRole}@taskit.com";
        string demoPw = "Pa$$w0rd";
        string returnUrl = Url.Content("~/");

        var user = await _signInManager.UserManager.FindByEmailAsync(demoEmail);
        if (user == null)
        {
            ModelState.AddModelError(String.Empty, "Invalid login attempt.");
            return RedirectToAction("Home", "Index");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, demoPw, false);
        if (result.Succeeded)
        {
            var claims = new List<Claim>
            {
                new Claim("amr", "pwd"),
            };

            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var roleClaim = string.Join(",", roles);
                claims.Add(new Claim("Roles", roleClaim));
            }

            await _signInManager.SignInWithClaimsAsync(user, false, claims);
            return LocalRedirect(returnUrl);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return RedirectToAction("Home", "Index");
        }
    }

}