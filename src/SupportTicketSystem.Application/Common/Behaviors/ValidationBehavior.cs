using FluentValidation;
using MediatR;

namespace SupportTicketSystem.Application.Common.Behaviors;

// Bu sınıf, MediatR üzerinden geçen her isteği yakalar ve eğer o istek için yazılmış bir validator varsa çalıştırır.
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(); // Validator yoksa doğrudan devam et
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            // Hata varsa FluentValidation Exception fırlat (Bunu GlobalExceptionHandler yakalayacak)
            throw new ValidationException(failures);
        }

        return await next(); // Hata yoksa işlemi sürdür
    }
}
