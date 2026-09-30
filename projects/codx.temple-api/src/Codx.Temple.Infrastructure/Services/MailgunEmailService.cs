using System.Net.Http.Headers;
using System.Text;
using Codx.Temple.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Codx.Temple.Infrastructure.Services;

public class MailgunEmailService : IEmailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MailgunEmailService> _logger;

    public MailgunEmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<MailgunEmailService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var apiKey = _configuration["Email:ApiKey"];
        var domain = _configuration["Email:Domain"];
        var from = _configuration["Email:From"] ?? "no-reply@templecourts.local";

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(domain))
        {
            _logger.LogInformation("[EMAIL][fallback] To: {To}, Subject: {Subject}, Body: {Body}", to, subject, body);
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Mailgun");
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{domain}/messages");
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{apiKey}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("from", from),
                new KeyValuePair<string, string>("to", to),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("text", body),
            });

            var response = await client.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("[EMAIL] Mailgun returned {StatusCode}: {Body}", (int)response.StatusCode, errorBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EMAIL] Failed to send email to {To}", to);
        }
    }
}
