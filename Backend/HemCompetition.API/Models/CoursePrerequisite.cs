using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CoursePrerequisite
{
    public int CourseId { get; set; }

    public int PrerequisiteId { get; set; }
}
