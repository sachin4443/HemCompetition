using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblStudentMark
{
    public int MarkId { get; set; }

    public int InstituteId { get; set; }

    public int StudentId { get; set; }

    public int ConfigId { get; set; }

    public decimal MarksObtained { get; set; }

    public bool? IsAbsent { get; set; }

    public string? Remarks { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedTimestamp { get; set; }

    public DateTime? ModifiedTimestamp { get; set; }

    public virtual HfTblExamConfiguration Config { get; set; } = null!;

    public virtual WjInstitute Institute { get; set; } = null!;

    public virtual WjStudent Student { get; set; } = null!;
}
