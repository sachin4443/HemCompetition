using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblAffliateFocusedList
{
    public int Id { get; set; }

    public int? InstituteId { get; set; }

    public int? AffliateId { get; set; }

    public int? EntId { get; set; }

    public DateTime? EntDate { get; set; }
}
