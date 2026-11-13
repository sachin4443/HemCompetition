using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblAffiliateKraFile
{
    public int FileId { get; set; }

    public int AffiliateId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int VersionNumber { get; set; }

    public DateTime UploadedAt { get; set; }

    public bool IsLatest { get; set; }

    public string? UploadLog { get; set; }
}
