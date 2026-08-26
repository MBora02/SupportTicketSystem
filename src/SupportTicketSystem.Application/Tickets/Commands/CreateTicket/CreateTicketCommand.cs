using MediatR;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority Priority,
    string CreatedBy) : IRequest<Guid>;
