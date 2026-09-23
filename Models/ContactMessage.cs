namespace GiftOfTheGivers.Models;

public class ContactMessage
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsHandled { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
