using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjGame24
{
    public int Game24Id { get; set; }

    public int? GameTopicId { get; set; }

    public string? Game24Level { get; set; }

    public int? GameNumber1 { get; set; }

    public int? GameNumber2 { get; set; }

    public int? GameNumber3 { get; set; }

    public int? GameNumber4 { get; set; }

    public int? GameAnswer { get; set; }

    public bool? GameIsActive { get; set; }

    public int? CourseId { get; set; }

    public string? CourseName { get; set; }

    public virtual WjGame24Topic? GameTopic { get; set; }
}
