using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.Application.Tickets.Commands.AddComment;
using SupportTicketSystem.Application.Tickets.Commands.CreateTicket;
using SupportTicketSystem.Application.Tickets.Queries.GetTicketById;
using SupportTicketSystem.Application.Tickets.Queries.GetTicketsList;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ISender _sender;

    public TicketsController(ISender sender)
    {
        _sender = sender;
    }

    // POST /api/tickets - Yeni Bilet Oluşturma
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketCommand command, CancellationToken cancellationToken)
    {
        var ticketId = await _sender.Send(command, cancellationToken);

        // 201 Created döner ve Response Header'a bu bilete erişilebilecek URL'i ekler (Location)
        return CreatedAtAction(nameof(GetById), new { id = ticketId }, ticketId);
    }

    // GET /api/tickets - Tüm Biletleri Listeleme
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tickets = await _sender.Send(new GetTicketsQuery(), cancellationToken);
        return Ok(tickets);
    }

    // GET /api/tickets/{id} - Tek Bilet Detayını Yorumlarıyla Getirme
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var ticket = await _sender.Send(new GetTicketByIdQuery(id), cancellationToken);

        if (ticket == null)
        {
            return NotFound(new { Message = $"Id'si {id} olan bilet bulunamadı." });
        }

        return Ok(ticket);
    }// POST /api/tickets/{id}/comments - Bilete Yorum Ekleme ve Durum Güncelleme
    // (Yeni eklediğimiz endpoint)
    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(
        [FromRoute] Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        // URL'den aldığımız 'id' ile Body'den aldığımız verileri birleştirip Command nesnesi üretiyoruz
        var command = new AddCommentCommand(id, request.Content, request.CreatedBy, request.NewStatus);

        var commentId = await _sender.Send(command, cancellationToken);

        return Ok(commentId);
    }
}
// API'ye bilet kimliği dışındaki bilgileri göndermek için kullandığımız sadeleştirilmiş istek gövdesi (Body) modeli
public record AddCommentRequest(
    string Content,
    string CreatedBy,
    TicketStatus? NewStatus
);
