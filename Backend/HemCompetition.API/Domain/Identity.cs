namespace HemCompetition.API.Domain;

public class AdminRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>System roles cannot be deleted; Super Admin cannot be edited.</summary>
    public bool IsSystem { get; set; }
    public List<AdminRolePermission> Permissions { get; set; } = new();
}

public class AdminRolePermission
{
    public int RoleId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public AdminRole Role { get; set; } = null!;
}

/// <summary>Rotating refresh token. Only the SHA-256 hash is stored.</summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public long? StudentId { get; set; }
    public Guid? AdminId { get; set; }
    /// <summary>Student session this token belongs to (auth_sessions.session_id).</summary>
    public Guid? SessionId { get; set; }
    /// <summary>All tokens descended from one login share a family; reuse of a rotated token revokes the family.</summary>
    public Guid FamilyId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}

public class Notification
{
    public long Id { get; set; }
    /// <summary>Null means the notification is addressed to every student.</summary>
    public long? StudentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = "info";
    public DateTime CreatedAtUtc { get; set; }
    public Guid? CreatedByAdminId { get; set; }
}

public class NotificationRead
{
    public long NotificationId { get; set; }
    public long StudentId { get; set; }
    public DateTime ReadAtUtc { get; set; }
}

public class AuditLog
{
    public long Id { get; set; }
    public string ActorType { get; set; } = "admin";
    public string? ActorId { get; set; }
    public string? ActorName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class SystemSetting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
