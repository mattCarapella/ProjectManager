﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class TicketIsSubmitterAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Ticket>
{
    UserManager<ApplicationUser> _userManager;
    private readonly TaskManagerContext _context;

    public TicketIsSubmitterAuthorizationHandler(UserManager<ApplicationUser> userManager, TaskManagerContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    OperationAuthorizationRequirement requirement,
                                                    Ticket resource)
    {
        if (context.User is null || resource is null)
        {
            return Task.CompletedTask;
        }

        // If not asking for CRUD permission, return
        if (requirement.Name != AuthConstants.CreateOperationName &&
            requirement.Name != AuthConstants.ReadOperationName &&
            requirement.Name != AuthConstants.UpdateOperationName)
        {
            return Task.CompletedTask;
        }
        var project = _context.Projects.AsNoTracking().FirstOrDefault(p => p.Id == resource.ProjectId);

        //if (resource.SubmittedById == _userManager.GetUserId(context.User) && 
        //    context.User.IsInRole(Constants.Roles.Manager))
        //{
        //    context.Succeed(requirement);
        //}

        if (resource.SubmittedById == _userManager.GetUserId(context.User) || 
            project is not null && project.CreatedByUserId == _userManager.GetUserId(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

