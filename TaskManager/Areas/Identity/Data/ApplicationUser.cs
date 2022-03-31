﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [StringLength(50, MinimumLength = 1)]
    [Required]
    public string FirstName { get; set; }

    [StringLength(50, MinimumLength = 1)]
    [Required]
    public string LastName { get; set; }

    public string Photo { get; set; } = "";

    public string? EmployeeID { get; set; }

    public string? JobTitle { get; set; }

    [Display(Name = "Full Name")]
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLoggedInAt  { get; set; }



    public ICollection<ProjectAssignment>? Projects { get; set; }

    public ICollection<TicketAssignment>? Tickets { get; set; }

}

public class ApplicationRole : IdentityRole
{
}