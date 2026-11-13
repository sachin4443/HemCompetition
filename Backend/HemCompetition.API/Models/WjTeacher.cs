using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjTeacher
{
    public int TeacherId { get; set; }

    public string? Standard { get; set; }

    public string? Division { get; set; }

    public string? ProfileUrl { get; set; }

    public virtual User Teacher { get; set; } = null!;

    public virtual ICollection<WjTeacherAssociate> WjTeacherAssociates { get; set; } = new List<WjTeacherAssociate>();

    public virtual ICollection<WjTeacherPracticalSubmission> WjTeacherPracticalSubmissions { get; set; } = new List<WjTeacherPracticalSubmission>();

    public virtual ICollection<WjTeachersCoin> WjTeachersCoins { get; set; } = new List<WjTeachersCoin>();
}
