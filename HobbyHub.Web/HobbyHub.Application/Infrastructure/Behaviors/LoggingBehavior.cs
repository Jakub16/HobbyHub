using MediatR;
using Serilog;

namespace HobbyHub.Application.Infrastructure.Behaviors;

public class LoggingBehavior<Trequest, TResponse> : IPipelineBehavior<Trequest, TResponse>
{
    private readonly ILogger _log;

    public LoggingBehavior(ILogger log)
    {
        _log = log;
    }

    public async Task<TResponse> Handle(Trequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            _log.Information($"Handling { typeof(Trequest).Name }.");

            var result = await next();
            
            _log.Information($"Handled { typeof(Trequest).Name }.");

            return result;
        }
        catch (Exception e)
        {
            _log.Error($"Exception while handling { typeof(Trequest).Name }.");
            throw;
        }
    }
}