using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblEmailLog
{
    public int LogId { get; set; }

    public string? RecipientEmail { get; set; }

    public string? Subject { get; set; }

    public string? MessageBody { get; set; }

    public string? Status { get; set; }

    public string? ResponseMessage { get; set; }

    public string? TemplateId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? SentDate { get; set; }

    public int? RetryCount { get; set; }

    public string? SentBy { get; set; }

    public string? ApplicationName { get; set; }

    public string? CcName { get; set; }
}
