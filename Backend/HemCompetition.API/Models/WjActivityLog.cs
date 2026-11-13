using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjActivityLog
{
    public int Id { get; set; }

    public string? ModuleName { get; set; }

    public int? ReferenceId { get; set; }

    public string? ReferenceId2 { get; set; }

    public string? Message { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ReturnUrl { get; set; }

    public string? Ipaddress { get; set; }

    public string? Browser { get; set; }

    public bool? SharingPermission { get; set; }
}
