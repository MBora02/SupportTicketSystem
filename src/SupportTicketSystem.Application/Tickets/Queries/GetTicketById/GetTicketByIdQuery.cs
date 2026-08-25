using Mapster;
using MediatR;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Repositories;

namespace SupportTicketSystem.Application.Tickets.Queries.GetTicketById;

// Query Tanımı: Hangi bileti getireceğini Id ile alır. Bilet bulunamazsa geriye null dönebilir.
public record GetTicketByIdQuery(Guid Id) : IRequest<TicketDetailDto?>;

// Detaylı Bilet DTO: Biletin kendisi ve yorumlarını içerir.
public record TicketDetailDto(
    Guid Id,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    string CreatedBy,
    string? AssignedTo,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<CommentDto> Comments);

// Yorum DTO: İlişkili yorum verileri.
public record CommentDto(
    Guid Id,
    string Content,
    string CreatedBy,
    DateTime CreatedAt);

// Handler
public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDetailDto?>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketByIdQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketDetailDto?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null)
        {
            return null;
        }

        // Mapster bilet altındaki Comments listesini de otomatik olarak CommentDto listesine eşler.
        return ticket.Adapt<TicketDetailDto>();
    }
}
