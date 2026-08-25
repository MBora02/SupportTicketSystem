using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SupportTicketSystem.WebApi.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Uygulamada beklenmeyen bir hata oluştu: {Message}", exception.Message);

        ProblemDetails problemDetails;

        // Eğer hata validasyon hatası ise (FluentValidation fırlattıysa)
        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Validasyon Hatası",
                Detail = "Gönderilen istek alan doğrulamasını geçemedi.",
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions.Add("errors", errors);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else
        {
            // Genel sistem hataları için (500 Internal Server Error)
            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Sunucu Hatası",
                Detail = "İşlem gerçekleştirilirken sunucuda beklenmedik bir hata oluştu.",
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Hatanın ele alındığını (handled) belirtir
    }
}
