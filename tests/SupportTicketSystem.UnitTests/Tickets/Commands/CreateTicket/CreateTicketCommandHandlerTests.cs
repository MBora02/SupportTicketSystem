using FluentAssertions;
using NSubstitute;
using SupportTicketSystem.Application.Tickets.Commands.CreateTicket;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Repositories;

namespace SupportTicketSystem.UnitTests.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandlerTests
{
    private readonly ITicketRepository _ticketRepositoryMock;
    private readonly CreateTicketCommandHandler _handler;

    public CreateTicketCommandHandlerTests()
    {
        // ITicketRepository arayüzünün taklidini (Mock) oluşturuyoruz
        _ticketRepositoryMock = Substitute.For<ITicketRepository>();

        // Taklit repository'nin AddAsync metodunun bilet nesnesine Guid atamasını sağlıyoruz (EF Core davranışını taklit eder)
        _ticketRepositoryMock.AddAsync(Arg.Any<Ticket>(), Arg.Any<CancellationToken>())
            .Returns(x => 
            {
                var ticket = (Ticket)x[0];
                ticket.Id = Guid.NewGuid();
                return Task.CompletedTask;
            });

        // Handler'ımıza bu taklit repository'yi enjekte ediyoruz
        _handler = new CreateTicketCommandHandler(_ticketRepositoryMock);
    }

    [Fact] // xUnit test metodu olduğunu belirtir
    public async Task Handle_Should_CreateTicketAndSaveToDatabase_WhenCommandIsValid()
    {
        // 1. Arrange (Hazırlık): Test girdilerimizi hazırlarız
        var command = new CreateTicketCommand(
            Title: "Test Başlığı",
            Description: "Test Açıklaması",
            Priority: TicketPriority.High,
            CreatedBy: "test@user.com"
        );

        // 2. Act (Eylem): Test etmek istediğimiz metodu çalıştırırız
        var result = await _handler.Handle(command, CancellationToken.None);

        // 3. Assert (Doğrulama): Sonuçların doğruluğunu denetleriz

        // Dönen Guid değerinin boş olmadığını doğrula
        result.Should().NotBeEmpty();

        // Repository'nin AddAsync metodunun tam 1 kez ve gönderdiğimiz command verileriyle çağrıldığını doğrula
        await _ticketRepositoryMock.Received(1).AddAsync(
            Arg.Is<Ticket>(t =>
                t.Title == command.Title &&
                t.Description == command.Description &&
                t.Priority == command.Priority &&
                t.CreatedBy == command.CreatedBy &&
                t.Status == TicketStatus.Open),
            Arg.Any<CancellationToken>()
        );

        // Değişikliklerin veritabanına kaydedilmesi için SaveChangesAsync metodunun 1 kez çağrıldığını doğrula
        await _ticketRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
