using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class UserActivityLog
{
    public int Id { get; set; }

    public string? ApplicationName { get; set; }

    public string? LogType { get; set; }

    public string? LogSubType { get; set; }

    public int? ReferenceId { get; set; }

    public string? Remark { get; set; }

    public string? Ipaddress { get; set; }

    public string? DeviceDetails { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UserName { get; set; }

    public string? HtmlLog { get; set; }

    public string? HtmlTemplate { get; set; }
}
