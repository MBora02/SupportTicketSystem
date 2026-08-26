using FluentAssertions;
using NSubstitute;
using SupportTicketSystem.Application.Tickets.Commands.AddComment;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Repositories;

namespace SupportTicketSystem.UnitTests.Tickets.Commands.AddComment;

public class AddCommentCommandHandlerTests
{
    private readonly ITicketRepository _ticketRepositoryMock;
    private readonly AddCommentCommandHandler _handler;


    public AddCommentCommandHandlerTests()
    {
        // Repository taklidini oluşturuyoruz
        _ticketRepositoryMock = Substitute.For<ITicketRepository>();

        // Handler'ı taklit repository ile kuruyoruz
        _handler = new AddCommentCommandHandler(_ticketRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_AddCommentAndChangeStatus_WhenTicketExists()
    {
        // 1. Arrange (Hazırlık)
        var ticketId = Guid.NewGuid();
        var existingTicket = new Ticket
        {
            Id = ticketId,
            Title = "Eski Başlık",
            Status = TicketStatus.Open
        };

        // Repository'nin GetByIdAsync metodu çağrıldığında bu bilet nesnesini dönmesini sağlıyoruz
        _ticketRepositoryMock.GetByIdAsync(ticketId, Arg.Any<CancellationToken>())
            .Returns(existingTicket);

        // Taklit repository'nin SaveChangesAsync çağrıldığında biletin yorumlarına otomatik Guid atamasını sağlıyoruz (EF Core davranışını taklit eder)
        _ticketRepositoryMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(x =>
            {
                foreach (var comment in existingTicket.Comments)
                {
                    if (comment.Id == Guid.Empty)
                    {
                        comment.Id = Guid.NewGuid();
                    }
                }
                return Task.CompletedTask;
            });

        var command = new AddCommentCommand(
            TicketId: ticketId,
            Content: "Test Yorumu",
            CreatedBy: "destek.uzmani@sirket.com",
            NewStatus: TicketStatus.InProgress
        );

        // 2. Act (Çalıştırma)
        var result = await _handler.Handle(command, CancellationToken.None);

        // 3. Assert (Doğrulama)
        result.Should().NotBeEmpty(); // Dönen yorum kimliği boş olmamalı

        // Biletin altına yorumun eklendiğini doğrula
        existingTicket.Comments.Should().HaveCount(1);
        existingTicket.Comments.First().Content.Should().Be(command.Content);
        existingTicket.Comments.First().CreatedBy.Should().Be(command.CreatedBy);

        // Bilet durumunun güncellendiğini doğrula
        existingTicket.Status.Should().Be(TicketStatus.InProgress);

        // Kaydetme metodunun çağrıldığını doğrula
        await _ticketRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenTicketDoesNotExist()
    {
        // 1. Arrange (Hazırlık)
        var ticketId = Guid.NewGuid();

        // Repository bilet arandığında null (bulunamadı) dönecek
        _ticketRepositoryMock.GetByIdAsync(ticketId, Arg.Any<CancellationToken>())
            .Returns((Ticket?)null);

        var command = new AddCommentCommand(
            TicketId: ticketId,
            Content: "Test Yorumu",
            CreatedBy: "destek.uzmani@sirket.com",
            NewStatus: null
        );

        // 2. Act (Çalıştırma) & Assert (Doğrulama)
        // Handler'ın KeyNotFoundException fırlatmasını bekliyoruz
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Id'si {ticketId} olan destek talebi bulunamadı.");

        // Bilet olmadığı için veritabanına kaydetme işlemi hiç çağrılmamış olmalı
        await _ticketRepositoryMock.Received(0).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
