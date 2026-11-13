using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjTechTeacherQuestion
{
    public int Id { get; set; }

    public int? TeacherId { get; set; }

    public int? QuestionId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Imagepath { get; set; }

    public bool? Status { get; set; }

    public bool? IsApp { get; set; }
}
