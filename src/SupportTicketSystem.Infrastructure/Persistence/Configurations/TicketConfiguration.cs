using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        // Primary Key tanımlama
        builder.HasKey(t => t.Id);

        // Kolon ayarları (Null olamaz ve maksimum uzunluk sınırları)
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.AssignedTo)
            .HasMaxLength(100);

        // Enum değerlerini veritabanına sayı (int) yerine metin (string) olarak kaydetmek
        // (Veritabanında doğrudan 'Open', 'InProgress' gibi okunabilmesi için çok yaygın bir yöntemdir)
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Bire-Çok İlişki (Bir biletin birden fazla yorumu olabilir, bilet silinirse yorumlar da silinir)
        builder.HasMany(t => t.Comments)
            .WithOne(c => c.Ticket)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
