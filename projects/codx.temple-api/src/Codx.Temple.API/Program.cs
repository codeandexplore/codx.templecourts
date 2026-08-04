using System.Text;
using Codx.Temple.API.Authorization;
using Codx.Temple.API.Data;
using Codx.Temple.API.Middleware;
using Codx.Temple.API.Serialization;
using Codx.Temple.Application;
using Codx.Temple.Infrastructure;
using Codx.Temple.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<CurrentUserAccessor>();
    builder.Services.AddScoped<Codx.Temple.Application.Abstractions.ICurrentUserAccessor>(sp =>
        sp.GetRequiredService<CurrentUserAccessor>());

    builder.Services.AddSingleton<RoleAwareJsonTypeResolver>();
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();

    builder.Services.AddOptions<Microsoft.AspNetCore.Mvc.JsonOptions>()
        .Configure<RoleAwareJsonTypeResolver>((options, resolver) =>
        {
            options.JsonSerializerOptions.TypeInfoResolver = resolver;
        });

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "")),
                RoleClaimType = "application_role"
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        foreach (var role in new[] { "Admin", "Teacher", "Student" })
        {
            options.AddPolicy($"RequireRole_{role}", policy =>
                policy.Requirements.Add(new RequireRoleRequirement(role)));
        }
    });

    builder.Services.AddScoped<IAuthorizationHandler, RequireRoleHandler>();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddSignalR();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<Codx.Temple.API.Hubs.StudySessionHub>("/hubs/study-session");
    app.MapHealthChecks("/healthz");
    app.MapHealthChecks("/readyz");

    await DataSeeder.SeedAsync(app.Services, builder.Configuration);

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program { }
