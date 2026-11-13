using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CfAnubhutiImagesUpload
{
    public int Qmid { get; set; }

    public DateTime? EntryDate { get; set; }

    public int? CourseId { get; set; }

    public int? Qid { get; set; }

    public byte[]? SmileImage { get; set; }

    public byte[]? SadImage { get; set; }

    public int? UserId { get; set; }
}
