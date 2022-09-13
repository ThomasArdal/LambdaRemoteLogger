using FastEndpoints;
using LambdaRemoteLogger.Lambda.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;

namespace LambdaRemoteLogger.Lambda.Endpoints;

[HttpPost("/log/error"), Authorize]
public class LogEndpoint : Endpoint<LogRequest>
{
    private readonly ILogger<LogEndpoint> logger;

    public LogEndpoint(ILogger<LogEndpoint> logger)
    {
        this.logger = logger;
    }

    public override async Task HandleAsync(LogRequest req, CancellationToken ct)
    {
        logger.LogInformation("HandleAsync called");
    }
}