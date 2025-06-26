using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using HobbyHub.Application.Infrastructure.Behaviors;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies;
using HobbyHub.Application.RequestHandlers.Marketplace;
using HobbyHub.Application.RequestHandlers.Tutorials;
using HobbyHub.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using StructureMap;

namespace HobbyHub.Application.Infrastructure;

[ExcludeFromCodeCoverage]
public class ApplicationRegistry : Registry
{
    public ApplicationRegistry(IConfiguration configuration)
    {
        RegisterMediatr();
        RegisterProviders();
        IncludeRegistry<InfrastructureRegistry>();
    }

    private void RegisterMediatr()
    {
        Scan(scanner =>
        {
            scanner.AssemblyContainingType<HobbyHubRequestHandler>();
            scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            scanner.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
        });

        For<IMediator>().Use<Mediator>();
        For<IHobbyHubRequestHandler>().Use<HobbyHubRequestHandler>();
        For(typeof(IPipelineBehavior<,>)).Add(typeof(LoggingBehavior<,>));
    }

    private void RegisterProviders()
    {
        For<IFavoriteHobbiesService>().Use<FavoriteHobbiesService>();
        For<IMarketplaceItemMapper>().Use<MarketplaceItemMapper>();
        For<ITutorialMapper>().Use<TutorialMapper>();
    }
}