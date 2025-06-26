using MediatR;

namespace HobbyHub.Application.Infrastructure.RequestHandling;

public interface IHobbyHubRequestHandler
{
    Task<TResponse> HandleRequest<TResponse>(IHobbyHubRequest<TResponse> request);
}

public class HobbyHubRequestHandler(IMediator mediator) : IHobbyHubRequestHandler
{
    public async Task<TResponse> HandleRequest<TResponse>(IHobbyHubRequest<TResponse> request)
    {
        return await mediator.Send(request);
    }
}