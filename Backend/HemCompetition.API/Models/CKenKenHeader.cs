using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CKenKenHeader
{
    public int Id { get; set; }

    public int Size { get; set; }

    public string? Operation { get; set; }

    public string? Difficulty { get; set; }

    public string? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? Title { get; set; }
}
