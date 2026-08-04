using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Codx.Temple.Application;

internal sealed class ApplicationMarker { }

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ApplicationMarker>();

        services.Scan(scan => scan
            .FromAssemblyOf<ApplicationMarker>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("UseCase")))
            .AsSelf()
            .WithScopedLifetime());

        return services;
    }
}
