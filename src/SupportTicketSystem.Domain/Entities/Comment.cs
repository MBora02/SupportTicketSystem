using SupportTicketSystem.Domain.Common;

namespace SupportTicketSystem.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid TicketId { get; set; } // Hangi talebe yazıldığını belirten Yabancı Anahtar (Foreign Key)
    public string Content { get; set; } = string.Empty; // Yorum içeriği
    public string CreatedBy { get; set; } = string.Empty; // Yorumu yazan kişi

    // Navigation Property: EF Core ilişkisi için üst nesneye referans
    public Ticket? Ticket { get; set; }
}
