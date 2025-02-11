﻿using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models
{
    public class TicketAssignment
    {
        public Guid TicketAssignmentId { get; set; }
        public Guid? TicketId { get; set; }
        public string? ApplicationUserId { get; set; }

        public Ticket? Ticket { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public string? AssignedByUsedId { get; set; }
    }
}
