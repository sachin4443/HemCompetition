using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class EventData2022
{
    public int Qrid { get; set; }

    public int UserId { get; set; }

    public byte[]? QrcodeImage { get; set; }

    public string? PhysicalQrcodePath { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsMailSent { get; set; }

    public bool? IsWhatsappSent { get; set; }

    public virtual User User { get; set; } = null!;
}
