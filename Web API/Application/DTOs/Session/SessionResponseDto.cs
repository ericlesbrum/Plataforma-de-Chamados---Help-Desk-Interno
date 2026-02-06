namespace Web_API.Application.DTOs.Session;

public class SessionResponseDto
{
    public int SessionId { get; set; }
    public string? DeviceId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsCurrent { get; set; }
}
