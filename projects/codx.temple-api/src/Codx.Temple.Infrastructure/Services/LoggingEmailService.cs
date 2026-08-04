using Codx.Temple.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Codx.Temple.Infrastructure.Services;

public class LoggingEmailService : IEmailService
{
    private readonly ILogger<LoggingEmailService> _logger;

    public LoggingEmailService(ILogger<LoggingEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogInformation("[EMAIL] To: {To}, Subject: {Subject}, Body: {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
