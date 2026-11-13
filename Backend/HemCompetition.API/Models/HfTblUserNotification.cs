using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblUserNotification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? NotificationId { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual HfTblNotification? Notification { get; set; }
}
