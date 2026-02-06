using Web_API.Domain.Enums;

namespace Web_API.Domain.Entities;

public class SecurityEvent
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public SecurityEventTypeEnum EventType { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
