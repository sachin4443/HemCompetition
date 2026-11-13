using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblMenu
{
    public int MenuId { get; set; }

    public string? MenuName { get; set; }

    public decimal? MenuCode { get; set; }

    public decimal? ParentMenuCode { get; set; }

    public string? Url { get; set; }

    public string? Target { get; set; }

    public string? Icon { get; set; }

    public int? MenuLevel { get; set; }

    public bool? Isactive { get; set; }

    public int? Level { get; set; }

    public int? ParentMenuCodeCore { get; set; }

    public bool? IscoreMenu { get; set; }

    public string? CoreMenuUrl { get; set; }
}
