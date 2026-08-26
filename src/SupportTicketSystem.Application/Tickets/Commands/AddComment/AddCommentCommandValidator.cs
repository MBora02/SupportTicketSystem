using FluentValidation;

namespace SupportTicketSystem.Application.Tickets.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        // TicketId boş (00000000-0000...) olamaz
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Yorum yapılacak biletin kimliği (TicketId) boş olamaz.");

        // Yorum içeriği boş olamaz ve en fazla 500 karakter olabilir
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Yorum içeriği boş olamaz.")
            .MaximumLength(500).WithMessage("Yorum içeriği en fazla 500 karakter olabilir.");

        // Yorumu yazan kişi boş olamaz ve geçerli bir e-posta adresi olmalıdır
        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Yorum yazan kullanıcı bilgisi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girilmelidir.");

        // Eğer yeni bilet durumu gönderildiyse, bunun geçerli bir enum değeri olduğunu doğrula
        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Geçerli bir bilet durumu seçilmelidir.")
            .When(x => x.NewStatus.HasValue); // Sadece NewStatus doluyken bu kuralı çalıştır
    }
}
