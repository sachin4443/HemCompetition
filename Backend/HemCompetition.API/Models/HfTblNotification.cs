using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblNotification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string TargetType { get; set; } = null!;

    public string? TargetValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<HfTblUserNotification> HfTblUserNotifications { get; set; } = new List<HfTblUserNotification>();
}
