using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Domain.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    void Update(Ticket ticket);
    void Delete(Ticket ticket);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
