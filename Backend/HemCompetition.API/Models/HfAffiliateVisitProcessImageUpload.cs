using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfAffiliateVisitProcessImageUpload
{
    public int VlImgId { get; set; }

    public DateTime? EntryDatetTime { get; set; }

    public int? ProcessId { get; set; }

    public byte[]? SchoolVisitImage { get; set; }

    public byte[]? TeacherVisitImage { get; set; }

    public byte[]? StudentVisitImage { get; set; }

    public int? UploadByAffiliatedUserId { get; set; }
}
