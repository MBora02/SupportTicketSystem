using MediatR;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Repositories;

namespace SupportTicketSystem.Application.Tickets.Commands.CreateTicket;

// Command: API'den gelecek verileri taşır ve geriye Guid (bilet id) döneceğini belirtir.
public record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority Priority,
    string CreatedBy) : IRequest<Guid>;

// Handler: İş mantığının (Business Logic) çalıştığı yerdir.
public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;

    public CreateTicketCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            CreatedBy = request.CreatedBy,
            Status = TicketStatus.Open // Yeni biletler 'Open' olarak başlar
        };

        // Veritabanına ekleme ve kaydetme işlemleri
        await _ticketRepository.AddAsync(ticket, cancellationToken);
        await _ticketRepository.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
