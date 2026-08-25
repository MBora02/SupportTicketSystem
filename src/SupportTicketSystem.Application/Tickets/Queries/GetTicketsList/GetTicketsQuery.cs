using Mapster;
using MediatR;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Repositories;

namespace SupportTicketSystem.Application.Tickets.Queries.GetTicketsList;

// Query Tanımı: Geriye TicketDto listesi döneceğini belirtir.
public record GetTicketsQuery() : IRequest<IEnumerable<TicketDto>>;

// Ticket DTO: Listeleme sayfasında gösterilecek sadeleştirilmiş veri yapısı.
public record TicketDto(
    Guid Id,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    string CreatedBy,
    string? AssignedTo,
    DateTime CreatedAt);

// Handler: Verileri repository'den çeker ve DTO'ya dönüştürür.
public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketsQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<IEnumerable<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetAllAsync(cancellationToken);

        // Mapster kullanarak Ticket listesini otomatik olarak TicketDto listesine dönüştürüyoruz.
        return tickets.Adapt<IEnumerable<TicketDto>>();
    }
}
