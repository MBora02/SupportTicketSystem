using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Repositories;

namespace SupportTicketSystem.Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Biletle birlikte biletin yorumlarını da çekiyoruz (Eager Loading)
        return await _context.Tickets
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // En yeni biletler en üstte görünecek şekilde sıralayıp getiriyoruz
        return await _context.Tickets
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
    }

    public void Update(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
    }

    public void Delete(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
