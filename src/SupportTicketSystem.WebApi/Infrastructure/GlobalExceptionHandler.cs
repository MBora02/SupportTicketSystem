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
        _logger.LogError(exception, "Uygulamada bir hata oluştu: {Message}", exception.Message);

        ProblemDetails problemDetails;

        // 1. Validasyon Hatası (400 Bad Request)
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
        // 2. Kaynak Bulunamadı Hatası (404 Not Found) - YENİ EKLEDİĞİMİZ KISIM
        else if (exception is KeyNotFoundException keyNotFoundException)
        {
            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Kaynak Bulunamadı",
                Detail = keyNotFoundException.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        // 3. Genel Sistem Hatası (500 Internal Server Error)
        else
        {
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

        return true;
    }
}
