using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblMenuActionMap
{
    public int Id { get; set; }

    public int? MenuId { get; set; }

    public string? ControllerName { get; set; }

    public string? ActionName { get; set; }
}
