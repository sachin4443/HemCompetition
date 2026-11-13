using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjStudentTeacherAssociation
{
    public int Id { get; set; }

    public int? TeacherId { get; set; }

    public int? StudentId { get; set; }
}
