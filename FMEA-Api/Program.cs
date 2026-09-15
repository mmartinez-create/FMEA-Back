using FMEA_Api.Middleware;
using FMEA_Api.Security;
using FmeaManager.Application;
using FmeaManager.Infrastructure;
using FmeaManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string DevelopmentCorsPolicy = "DevelopmentFrontend";

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserAccessor>();
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        DevelopmentCorsPolicy,
        policy => policy
            .WithOrigins(
                "http://localhost:4200",
                "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    if (app.Configuration.GetValue<bool>("Database:AutoMigrate"))
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<FmeaManagerDbContext>();

        app.Logger.LogInformation(
            "Applying FMEA Manager database migrations for Development...");

        await dbContext.Database.MigrateAsync();

        app.Logger.LogInformation(
            "Database migrations completed successfully.");
    }

    app.UseCors(DevelopmentCorsPolicy);
}
else
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
