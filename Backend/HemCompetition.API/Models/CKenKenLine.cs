using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CKenKenLine
{
    public int Id { get; set; }

    public int HeaderId { get; set; }

    public int RowNo { get; set; }

    public string? LineType { get; set; }

    public string? Column1 { get; set; }

    public string? Column2 { get; set; }

    public string? Column3 { get; set; }

    public string? Column4 { get; set; }

    public string? Column5 { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? Column6 { get; set; }

    public string? Column7 { get; set; }

    public string? Column8 { get; set; }

    public string? Column9 { get; set; }

    public string? Column10 { get; set; }
}
