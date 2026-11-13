using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ExaminationOption
{
    public int Id { get; set; }

    public int ExaminationId { get; set; }

    public int OptionId { get; set; }

    public int? DisplayOrder { get; set; }

    public virtual Examination Examination { get; set; } = null!;
}
