using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjFileUpload
{
    public int FileUploadId { get; set; }

    public int UserId { get; set; }

    public string? DocumentName { get; set; }

    public string? DocumentPath { get; set; }

    public string? DocumentType { get; set; }

    public bool? IsActive { get; set; }

    public bool? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
