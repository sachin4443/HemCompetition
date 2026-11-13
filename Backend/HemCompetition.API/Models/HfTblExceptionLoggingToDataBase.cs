using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblExceptionLoggingToDataBase
{
    public long Logid { get; set; }

    public string? ExceptionMsg { get; set; }

    public string? ExceptionType { get; set; }

    public string? ExceptionSource { get; set; }

    public string? ExceptionUrl { get; set; }

    public DateTime? Logdate { get; set; }

    public int? UserId { get; set; }

    public int? FeatureId { get; set; }

    public string? ProjectName { get; set; }

    public string? Methods { get; set; }

    public string? OthererrorMessage { get; set; }

    public string? IpAddress { get; set; }

    public string? HttpXForwardedFor { get; set; }
}
