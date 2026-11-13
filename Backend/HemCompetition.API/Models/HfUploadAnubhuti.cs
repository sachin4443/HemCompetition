using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfUploadAnubhuti
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int? PracticalId { get; set; }

    public byte[]? UploadedFiles { get; set; }

    public DateTime? IsCreatedDate { get; set; }

    public int? IsCreatedBy { get; set; }

    public int? IsModifiledBy { get; set; }

    public string? IsModifiledDate { get; set; }

    public string? FileName { get; set; }

    public string? FileExtension { get; set; }

    public string? FileLink { get; set; }

    public string? Status { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CourseName { get; set; }

    public string? Title { get; set; }
}
