using FmeaManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FMEA_Api.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    private static readonly string[] ExpectedTables =
    {
        "CustomerProfiles",
        "Plants",
        "ProductionLines",
        "Products",
        "Projects",
        "PermissionAssignments",
        "ProjectAuditEvents",
        "ProductProcesses",
        "ProductProcessSteps",
        "Fmeas",
        "FmeaRevisions",
        "ProcessSteps",
        "FailureModes",
        "FailureEffects",
        "FailureCauses",
        "PreventionControls",
        "DetectionControls",
        "RiskAssessments",
        "ControlPlans",
        "ControlPlanItems"
    };

    private readonly FmeaManagerDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public HealthController(
        FmeaManagerDbContext dbContext,
        IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(
            cancellationToken);

        if (!canConnect)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = "offline",
                    api = "FMEA Manager API",
                    environment = _environment.EnvironmentName,
                    databaseConnected = false,
                    missingTables = ExpectedTables
                });
        }

        var existingTables = await ReadExistingTablesAsync(
            cancellationToken);

        var missingTables = ExpectedTables
            .Where(table => !existingTables.Contains(table))
            .OrderBy(table => table)
            .ToArray();

        var healthy = missingTables.Length == 0;

        return Ok(
            new
            {
                status = healthy ? "ok" : "degraded",
                api = "FMEA Manager API",
                environment = _environment.EnvironmentName,
                databaseConnected = true,
                missingTables,
                utc = DateTime.UtcNow
            });
    }

    private async Task<HashSet<string>> ReadExistingTablesAsync(
        CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE';
                """;

            var tables = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            await using var reader = await command.ExecuteReaderAsync(
                cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                if (!reader.IsDBNull(0))
                {
                    tables.Add(reader.GetString(0));
                }
            }

            return tables;
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }
}
