using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjMainCertificate
{
    public int CertificateId { get; set; }

    public int? InstituteId { get; set; }

    public int QuizId { get; set; }

    public int CourseId { get; set; }

    public byte[]? Certificate { get; set; }

    public virtual WjInstitute? Institute { get; set; }
}
