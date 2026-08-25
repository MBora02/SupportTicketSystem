using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketSystem.Application.Common.Behaviors;

namespace SupportTicketSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR servisini ve içindeki tüm Handler'ları kaydeder, ayrıca validation pipeline'ı ekler
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>)); // Pipeline Behavior Kaydı
        });
        // FluentValidation içindeki tüm Validator'ları kaydeder
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
