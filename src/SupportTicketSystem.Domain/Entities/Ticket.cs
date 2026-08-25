using SupportTicketSystem.Domain.Common;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Domain.Entities;

public class Ticket : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public string CreatedBy { get; set; } = string.Empty; // Talebi açan kişi (E-posta veya isim)
    public string? AssignedTo { get; set; } // Atanan destek uzmanı

    // Navigation Property: EF Core için ilişkisel veri
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
