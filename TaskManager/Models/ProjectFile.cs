﻿using System.ComponentModel.DataAnnotations;
using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models;

public class ProjectFile
{
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    [Required]
    public byte[] Data { get; set; } = Array.Empty<byte>();

    [Required, StringLength(50)]
    public string FileType { get; set; }

    [Required, StringLength(10)]
    public string Extension { get; set; }

    [Required, StringLength(300)]
    public string Description { get; set; }

    public long FileSize { get; set; }

    public string UploadedByUserId { get; set; } = String.Empty;
    public ApplicationUser UploadedByUser { get; set; }

    public Guid ProjectId { get; set; }
    public Project? Project  { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
