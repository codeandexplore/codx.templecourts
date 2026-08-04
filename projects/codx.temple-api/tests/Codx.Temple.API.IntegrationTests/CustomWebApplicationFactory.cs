using Codx.Temple.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Codx.Temple.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("templecourts_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public HttpClient CreateAuthenticatedClient(Guid userId, string email, string displayName, params string[] roles)
    {
        var client = CreateClient();
        var token = JwtHelper.GenerateAccessToken(userId, email, displayName, roles);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public HttpClient CreateAdminClient()
    {
        return CreateAuthenticatedClient(Guid.NewGuid(), "admin@test.com", "Admin", "Admin");
    }

    public HttpClient CreateStudentClient()
    {
        return CreateAuthenticatedClient(Guid.NewGuid(), "student@test.com", "Student", "Student");
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString() + ";Include Error Detail=True");
            });
        });

        builder.UseSetting("Jwt:Secret", "test-secret-key-thats-long-enough-for-hs256-minimum-32-bytes!!");
        builder.UseSetting("Jwt:Issuer", "templecourts-test");
        builder.UseSetting("Jwt:Audience", "templecourts-test");
        builder.UseSetting("Jwt:AccessTokenExpiryMinutes", "60");
        builder.UseSetting("Jwt:RefreshTokenExpiryDays", "1");
    }
}
