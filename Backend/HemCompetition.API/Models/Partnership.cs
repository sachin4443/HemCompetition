using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Partnership
{
    public int PartnershipId { get; set; }

    public string Name { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Company { get; set; }

    public string? Work { get; set; }

    public bool IsActive { get; set; }

    public int CreatedById { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public int? EditedById { get; set; }

    public string? EditedBy { get; set; }

    public DateTime? EditedOn { get; set; }

    public string Status { get; set; } = null!;
}
