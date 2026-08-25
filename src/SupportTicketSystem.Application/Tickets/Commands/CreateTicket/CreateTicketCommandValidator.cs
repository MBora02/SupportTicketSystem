using FluentValidation;

namespace SupportTicketSystem.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Bilet başlığı boş olamaz.")
            .MaximumLength(150).WithMessage("Bilet başlığı en fazla 150 karakter olabilir.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama alanı boş olamaz.")
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Talebi açan kullanıcı bilgisi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girilmelidir.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Geçerli bir öncelik derecesi seçilmelidir.");
    }
}
