using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjTaskStudent
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int StudentId { get; set; }

    public bool? Iscompleted { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public bool? CanShow { get; set; }
}
