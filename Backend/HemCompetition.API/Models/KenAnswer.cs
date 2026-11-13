using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenAnswer
{
    public int Id { get; set; }

    public int? Kmid { get; set; }

    public int? CourseId { get; set; }

    public int? IsCreatedBy { get; set; }

    public DateTime? SubmitTime { get; set; }
}
