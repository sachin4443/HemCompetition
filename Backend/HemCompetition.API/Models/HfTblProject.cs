using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblProject
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? CreatedByUserId { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual ICollection<HfTblTask> HfTblTasks { get; set; } = new List<HfTblTask>();
}
