using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CourseCategory
{
    public int CategoryId { get; set; }

    public int CourseId { get; set; }

    public virtual CourseCategoryLookup Category { get; set; } = null!;
}
