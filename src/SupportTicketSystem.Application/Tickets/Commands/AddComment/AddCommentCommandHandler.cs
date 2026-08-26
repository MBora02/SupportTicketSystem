using MediatR;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Repositories;

namespace SupportTicketSystem.Application.Tickets.Commands.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;

    public AddCommentCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Guid> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        // 1. Veritabanından bileti yorumlarıyla birlikte çekiyoruz
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

        // 2. Eğer bilet bulunamazsa hata fırlatıyoruz
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Id'si {request.TicketId} olan destek talebi bulunamadı.");
        }

        // 3. Yeni bir Yorum (Comment) nesnesi oluşturuyoruz
        var comment = new Comment
        {
            Content = request.Content,
            CreatedBy = request.CreatedBy,
            TicketId = request.TicketId
        };

        // 4. Biletin altındaki Comments koleksiyonuna bu yorumu ekliyoruz (İşte Aggregate Root mantığı!)
        ticket.Comments.Add(comment);

        // 5. Eğer yeni bir bilet durumu gönderildiyse bilet durumunu güncelliyoruz
        if (request.NewStatus.HasValue)
        {
            ticket.Status = request.NewStatus.Value;
            ticket.UpdatedAt = DateTime.UtcNow; // Güncelleme tarihini güncelliyoruz
        }

        // 6. Değişiklikleri veritabanına kaydediyoruz.
        // EF Core biletin durumunun değiştiğini ve altına yeni bir yorum eklendiğini anlayıp
        // hem Ticket tablosunu güncelleyecek hem de Comment tablosuna INSERT yapacaktır.
        await _ticketRepository.SaveChangesAsync(cancellationToken);

        // 7. Oluşturulan yorumun Id'sini geri dönüyoruz
        return comment.Id;
    }
}
