using MediatR;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Tickets.Commands.AddComment;

// Command: API'den (Frontend'den) gelecek olan istek verilerini taşır.
// IRequest<Guid> ifadesi, bu işlem bittiğinde geriye oluşturulan Yorum'un Id'sini (Guid) döneceğimizi belirtir.
public record AddCommentCommand(
    Guid TicketId,      // Hangi bilete yorum yapılacak
    string Content,     // Yorum içeriği
    string CreatedBy,   // Yorumu yazan kişinin e-postası
    TicketStatus? NewStatus // Biletin güncellenecek yeni durumu (Seçimliktir, null gelebilir)
) : IRequest<Guid>;
