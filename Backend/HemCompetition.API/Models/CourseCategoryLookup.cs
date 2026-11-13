using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CourseCategoryLookup
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<CourseCategory> CourseCategories { get; set; } = new List<CourseCategory>();
}
