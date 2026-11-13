using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblTask
{
    public int TaskId { get; set; }

    public int ProjectId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? StatusId { get; set; }

    public string? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public int? CreatedByUserId { get; set; }

    public int? AssignedToUserId { get; set; }

    public virtual User? AssignedToUser { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual HfTblProject Project { get; set; } = null!;
}
