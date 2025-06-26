using MediatR;

namespace HobbyHub.Application.Infrastructure.RequestHandling;

public interface IHobbyHubRequest<out TResponse> : IRequest<TResponse>
{
    
}