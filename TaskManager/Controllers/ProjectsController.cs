﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Authorization;
using TaskManager.Core;
using TaskManager.Core.Enums;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;
using TaskManager.Core.ViewModels.Project;
using TaskManager.Data;
using TaskManager.Models;
using Constants = TaskManager.Core.Constants;

namespace TaskManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public ProjectsController(TaskManagerContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        // GET: Projects
        // Gets all projects assigned to a user
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CreatedOnSortParam"] = sortOrder == "createdOn" ? "createdOn_desc" : "createdOn";
            ViewData["NameSortParam"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["OpenTicketSortParam"] = sortOrder == "openTickets" ? "openTickets_desc" : "openTickets";

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var projects = await _unitOfWork.ProjectRepository.GetProjectsWithTicketsForUser(User.Identity.GetUserId());

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name":
                    projects = projects.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    projects = projects.OrderByDescending(p => p.Name);
                    break;
                case "createdOn_desc":
                    projects = projects.OrderByDescending(p => p.CreatedAt);
                    break;
                case "goalDate":
                    projects = projects.OrderBy(p => p.GoalDate);
                    break;
                case "goalDate_desc":
                    projects = projects.OrderByDescending(p => p.GoalDate);
                    break;
                case "openTickets":
                    projects = projects.OrderBy(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                case "openTickets_desc":
                    projects = projects.OrderByDescending(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                default:
                    projects = projects.OrderBy(p => p.CreatedAt);
                    break;
            }

            var pList = projects.ToList();
            int pageSize = 10;
            return View(PaginatedList<Project>.Create(pList, pageNumber ?? 1, pageSize));
        }


        // GET: AllProjects
        // Gets all projects
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> AllProjects(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CreatedOnSortParam"] = sortOrder == "createdOn" ? "createdOn_desc" : "createdOn";
            ViewData["NameSortParam"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["OpenTicketSortParam"] = sortOrder == "openTickets" ? "openTickets_desc" : "openTickets";

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var projects = await _unitOfWork.ProjectRepository.GetProjectsWithTickets();

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name":
                    projects = projects.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    projects = projects.OrderByDescending(p => p.Name);
                    break;
                case "createdOn_desc":
                    projects = projects.OrderByDescending(p => p.CreatedAt);
                    break;
                case "goalDate":
                    projects = projects.OrderBy(p => p.GoalDate);
                    break;
                case "goalDate_desc":
                    projects = projects.OrderByDescending(p => p.GoalDate);
                    break;
                case "openTickets":
                    projects = projects.OrderBy(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                case "openTickets_desc":
                    projects = projects.OrderByDescending(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                default:
                    projects = projects.OrderBy(p => p.CreatedAt);
                    break;
            }

            var pList = projects.ToList();
            int pageSize = 10;
            return View(PaginatedList<Project>.Create(pList, pageNumber ?? 1, pageSize));      
        }


        // GET: Manager
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> Manager(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CreatedOnSortParam"] = sortOrder == "createdOn" ? "createdOn_desc" : "createdOn";
            ViewData["NameSortParam"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["OpenTicketSortParam"] = sortOrder == "openTickets" ? "openTickets_desc" : "openTickets";

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var projectList = await _unitOfWork.ProjectRepository.GetProjectsForManager(User.Identity.GetUserId());
            var projects = from p in projectList select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name":
                    projects = projects.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    projects = projects.OrderByDescending(p => p.Name);
                    break;
                case "createdOn_desc":
                    projects = projects.OrderByDescending(p => p.CreatedAt);
                    break;
                case "goalDate":
                    projects = projects.OrderBy(p => p.GoalDate);
                    break;
                case "goalDate_desc":
                    projects = projects.OrderByDescending(p => p.GoalDate);
                    break;
                case "openTickets":
                    projects = projects.OrderBy(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                case "openTickets_desc":
                    projects = projects.OrderByDescending(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                default:
                    projects = projects.OrderBy(p => p.CreatedAt);
                    break;
            }

            var pList = projects.ToList();
            int pageSize = 10;

            var vm = new ProjectIndexViewModel
            {
                Projects = PaginatedList<Project>.Create(pList, pageNumber ?? 1, pageSize),
            };

            return View(vm);
        }



        // GET: Projects/Details/{id}
        public async Task<IActionResult> Details(Guid id, string? sortOrder, int? pageNumber)
        {
            if (id == Guid.Empty)  return NotFound();

            var project = await _unitOfWork.ProjectRepository.GetProjectWithTicketsNotesUsers(id);
            if (project is null) return NotFound();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["TicketTitleSortParam"] = sortOrder == "ticketTitle" ? "ticketTitle_desc" : "ticketTitle";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["StatusSortParam"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["PrioritySortParam"] = sortOrder == "priority" ? "priority_desc" : "priority";
            ViewData["TicketTypeSortParam"] = sortOrder == "ticketType" ? "ticketType_desc" : "ticketType";
            ViewData["ClosedAtSortParam"] = sortOrder == "closedAt" ? "closedAt_desc" : "closedAt";
            //ViewData["CurrentFilter"] = searchString;

            var tickets = from t in project.Tickets select t;
            switch (sortOrder)
            {
                case "ticketTitle":
                    tickets = tickets.OrderBy(t => t.Title);
                    break;
                case "ticketTitle_desc":
                    tickets = tickets.OrderByDescending(t => t.Title);
                    break;
                case "goalDate":
                    tickets = tickets.OrderBy(t => t.GoalDate);
                    break;
                case "goalDate_desc":
                    tickets = tickets.OrderByDescending(t => t.GoalDate);
                    break;
                case "status":
                    tickets = tickets.OrderBy(t => t.Status);
                    break;
                case "status_desc":
                    tickets = tickets.OrderByDescending(t => t.Status);
                    break;
                case "priority":
                    tickets = tickets.OrderBy(t => t.Priority);
                    break;
                case "priority_desc":
                    tickets = tickets.OrderByDescending(t => t.Priority);
                    break;
                case "ticketType":
                    tickets = tickets.OrderBy(t => t.TicketType);
                    break;
                case "ticketType_desc":
                    tickets = tickets.OrderByDescending(t => t.TicketType);
                    break;
                case "closedAt":
                    tickets = tickets.OrderBy(t => t.ClosedAt);
                    break;
                case "closedAt_desc":
                    tickets = tickets.OrderByDescending(t => t.ClosedAt);
                    break;
                default:
                    tickets = tickets.OrderBy(t => t.Status);
                    break;
            }

            var userTicketsForProj = new List<Ticket>();
            if (User.IsInRole("Administrator") || User.IsInRole("Manager"))
            {
                // Displays all tickets for the project
                userTicketsForProj = tickets.ToList();
            }
            else
            {
                userTicketsForProj = await _unitOfWork.TicketRepository.GetTicketsForProjectDetailsPage(project.Id, User.Identity.GetUserId());
            }

            var pageSize = 8;

            var openTickets = userTicketsForProj.Where(t => t.Status != Enums.Status.COMPLETED).ToList();
            var closedTickets = userTicketsForProj.Where(t => t.Status == Enums.Status.COMPLETED).ToList();

            var paginatedOpenTicketList = PaginatedList<Ticket>.Create(openTickets, pageNumber ?? 1, pageSize);
            var paginatedClosedTicketList = PaginatedList<Ticket>.Create(closedTickets, pageNumber ?? 1, pageSize);

            var vm = new ProjectDetailsViewModel()
            {
                Project = project,
                Contributers = project.Contributers!.ToList(),    
                ClosedTicketsPaginated = paginatedClosedTicketList,
                OpenTicketsPaginated = paginatedOpenTicketList,
                Notes = project.Notes,
                Files = project.ProjectFiles
            };

            return View(vm);
        }


        // GET: Projects/Create
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Projects/Create
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Tag,GoalDate")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await _unitOfWork.UserRepository.GetCurrentUser();
                    var contributer = new ProjectAssignment
                    {
                        ProjectAssignmentId = Guid.NewGuid(),
                        ApplicationUser = currentUser,
                        Project = project,
                        IsManager = true,
                        AssignedByUsedId = currentUser.Id
                    };

                    project.Id = Guid.NewGuid();
                    project.Contributers!.Add(contributer);
                    project.CreatedByUserId = currentUser.Id;
                    await _unitOfWork.ProjectRepository.AddProject(project);
                    await _unitOfWork.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save project.");
            }
            
            return View(project);
        }


        // GET: Projects/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var project = await _unitOfWork.ProjectRepository.GetProject(id);
            if (project is null) return NotFound();

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, project, TaskOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(project);
        }


        // POST: Projects/Edit/{id}
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var projectToUpdate = await _unitOfWork.ProjectRepository.GetProject(id, true);
            if (projectToUpdate is null) return NotFound();
            
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, projectToUpdate, TaskOperations.Update);
            if (!isAuthorized.Succeeded) return Forbid();

            if (await TryUpdateModelAsync<Project>(
                projectToUpdate, "", p => p.Name, p => p.Description, p => p.Tag, p => p.GoalDate))
            {
                try
                {
                    projectToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to update project.");
                }
            }
            return View(projectToUpdate);
        }


        // GET: Projects/Delete/{id}
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> Delete(Guid id, bool? saveChangeErrors = false)
        {
            if (id == Guid.Empty) return NotFound();

            var project = await _unitOfWork.ProjectRepository.GetProject(id, false);
            if (project is null) return NotFound();

            if (saveChangeErrors.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Project deletion failed.";
            }

            return View(project);
        }


        // POST: Projects/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await _unitOfWork.ProjectRepository.GetProject(id);
            if (project is null) return NotFound();

            try
            {
                await _unitOfWork.ProjectRepository.DeleteProject(project.Id);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


        // GET: ManageUsers/{id}
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> ManageUsers(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var project = await _unitOfWork.ProjectRepository.GetProjectWithUsers(id);
            if (project is null) return NotFound();

            var vm = GetViewModel(project);
            return View(vm);
        }


        // POST: ManageUsers/{id}
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUsers(Guid id, AddUserProjectViewModel projectViewModel)
        {
            var selectedUserId = projectViewModel.UserId;
            if (selectedUserId is null || id == Guid.Empty) return NotFound();

            var project = await _unitOfWork.ProjectRepository.GetProject(id);
            var selectedUser = await _unitOfWork.UserRepository.GetUserAsync(selectedUserId);
            var contributer = new ProjectAssignment
            {
                ProjectAssignmentId = Guid.NewGuid(),
                ApplicationUser = selectedUser,
                ApplicationUserId = selectedUserId,
                Project = project,
                ProjectId = project.Id,
                IsManager = false,
                AssignedByUsedId = User.Identity.GetUserId()
            };

            try
            {
                await _unitOfWork.ProjectAssignmentRepository.AddProjectAssignment(contributer); 
                await _unitOfWork.SaveAsync();
                return RedirectToAction("ManageUsers", "Projects", project);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to add user.");
            }

            var vm = GetViewModel(project);
            return View(vm);
        }


        // POST: RemoveUser
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> RemoveUser(Guid projectId, Guid paId)
        {
            if (paId == Guid.Empty || projectId == Guid.Empty) return NotFound();

            await _unitOfWork.ProjectAssignmentRepository.DeleteProjectAssignment(paId);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(ManageUsers), new { id = projectId });
        }


        private AddUserProjectViewModel GetViewModel(Project project)
        {
            /* Creates a dropdown list of users to add to a project. Users who are already contributers
               are removed from the dropdown here as well. */
            var inList = _context.ProjectAssignments
                                .Where(p => p.ProjectId == project.Id)
                                .AsNoTracking()
                                .Include(a => a.ApplicationUser);

            var notInList = _context.Users
                                .Where(a => !inList.Any(b => b.ApplicationUserId == a.Id))
                                .AsNoTracking()
                                .ToList();

            var userList = (from user in notInList
                            select new SelectListItem()
                            {
                                Text = user.FullName,
                                Value = user.Id.ToString()
                            }).ToList();

            userList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = String.Empty
            });

            ViewBag.ListOfUsers = userList.OrderBy(x => x.Text);
            var vm = new AddUserProjectViewModel
            {
                Project = project,
                ListOfUsers = userList
            };

            return vm;
        }

    }
}
