using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FmeaManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDay22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionLines_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Product = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Plant = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CustomerProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_CustomerProfiles_CustomerProfileId",
                        column: x => x.CustomerProfileId,
                        principalTable: "CustomerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ControlPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlPlans_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fmeas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fmeas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fmeas_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductProcesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductProcesses_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAuditEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorUserKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ActorDisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAuditEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAuditEvents_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FmeaRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FmeaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    BasedOnRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RevisionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FmeaRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FmeaRevisions_FmeaRevisions_BasedOnRevisionId",
                        column: x => x.BasedOnRevisionId,
                        principalTable: "FmeaRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FmeaRevisions_Fmeas_FmeaId",
                        column: x => x.FmeaId,
                        principalTable: "Fmeas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductProcessSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Function = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Requirement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProcessSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductProcessSteps_ProductProcesses_ProductProcessId",
                        column: x => x.ProductProcessId,
                        principalTable: "ProductProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ControlPlanItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductProcessStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessStepSequenceSnapshot = table.Column<int>(type: "int", nullable: false),
                    ProcessStepNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CharacteristicNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CharacteristicType = table.Column<int>(type: "int", nullable: false),
                    CharacteristicName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MachineTooling = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SpecialCharacteristic = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SpecificationTolerance = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EvaluationMeasurementTechnique = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SampleSize = table.Column<int>(type: "int", nullable: true),
                    SampleFrequency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ControlMethod = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReactionPlan = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlPlanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlPlanItems_ControlPlans_ControlPlanId",
                        column: x => x.ControlPlanId,
                        principalTable: "ControlPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ControlPlanItems_ProductProcessSteps_ProductProcessStepId",
                        column: x => x.ProductProcessStepId,
                        principalTable: "ProductProcessSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FmeaRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductProcessStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Function = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Requirement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessSteps_FmeaRevisions_FmeaRevisionId",
                        column: x => x.FmeaRevisionId,
                        principalTable: "FmeaRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessSteps_ProductProcessSteps_ProductProcessStepId",
                        column: x => x.ProductProcessStepId,
                        principalTable: "ProductProcessSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FailureModes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailureModes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailureModes_ProcessSteps_ProcessStepId",
                        column: x => x.ProcessStepId,
                        principalTable: "ProcessSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FailureCauses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailureModeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailureCauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailureCauses_FailureModes_FailureModeId",
                        column: x => x.FailureModeId,
                        principalTable: "FailureModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FailureEffects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailureModeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailureEffects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailureEffects_FailureModes_FailureModeId",
                        column: x => x.FailureModeId,
                        principalTable: "FailureModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetectionControls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailureCauseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectionControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetectionControls_FailureCauses_FailureCauseId",
                        column: x => x.FailureCauseId,
                        principalTable: "FailureCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreventionControls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailureCauseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreventionControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreventionControls_FailureCauses_FailureCauseId",
                        column: x => x.FailureCauseId,
                        principalTable: "FailureCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailureCauseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Occurrence = table.Column<int>(type: "int", nullable: false),
                    Detection = table.Column<int>(type: "int", nullable: false),
                    Rpn = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAssessments", x => x.Id);
                    table.CheckConstraint("CK_RiskAssessments_Detection", "[Detection] BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_RiskAssessments_Occurrence", "[Occurrence] BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_RiskAssessments_Rpn", "[Rpn] = [Severity] * [Occurrence] * [Detection]");
                    table.CheckConstraint("CK_RiskAssessments_Severity", "[Severity] BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_RiskAssessments_FailureCauses_FailureCauseId",
                        column: x => x.FailureCauseId,
                        principalTable: "FailureCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CustomerProfiles",
                columns: new[] { "Id", "Code", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "STANDARD", "Default customer-independent FMEA profile.", true, "Standard FMEA" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "FORD", "Initial Ford customer profile. Customer-specific rules will be refined in later increments.", true, "Ford" }
                });

            migrationBuilder.InsertData(
                table: "PermissionAssignments",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "Permissions", "ScopeId", "ScopeType", "UpdatedAt", "UpdatedBy", "UserKey" },
                values: new object[] { new Guid("4b1f9ee8-97f2-4f4a-94af-5fe5d08b6af7"), new DateTime(2026, 9, 10, 12, 0, 0, 0, DateTimeKind.Utc), "system-bootstrap", true, 7, new Guid("00000000-0000-0000-0000-000000000000"), 0, null, null, "local-development" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlPlanItems_ControlPlanId",
                table: "ControlPlanItems",
                column: "ControlPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlPlanItems_ControlPlanId_ProductProcessStepId",
                table: "ControlPlanItems",
                columns: new[] { "ControlPlanId", "ProductProcessStepId" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlPlanItems_ProductProcessStepId",
                table: "ControlPlanItems",
                column: "ProductProcessStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlPlans_Number",
                table: "ControlPlans",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_ControlPlans_ProjectId",
                table: "ControlPlans",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProfiles_Code",
                table: "CustomerProfiles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetectionControls_FailureCauseId",
                table: "DetectionControls",
                column: "FailureCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_FailureCauses_FailureModeId",
                table: "FailureCauses",
                column: "FailureModeId");

            migrationBuilder.CreateIndex(
                name: "IX_FailureEffects_FailureModeId",
                table: "FailureEffects",
                column: "FailureModeId");

            migrationBuilder.CreateIndex(
                name: "IX_FailureModes_ProcessStepId",
                table: "FailureModes",
                column: "ProcessStepId");

            migrationBuilder.CreateIndex(
                name: "IX_FmeaRevisions_BasedOnRevisionId",
                table: "FmeaRevisions",
                column: "BasedOnRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_FmeaRevisions_FmeaId",
                table: "FmeaRevisions",
                column: "FmeaId");

            migrationBuilder.CreateIndex(
                name: "IX_FmeaRevisions_FmeaId_RevisionNumber",
                table: "FmeaRevisions",
                columns: new[] { "FmeaId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fmeas_ProjectId",
                table: "Fmeas",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Fmeas_ProjectId_Number",
                table: "Fmeas",
                columns: new[] { "ProjectId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAssignments_UserKey_ScopeType_ScopeId",
                table: "PermissionAssignments",
                columns: new[] { "UserKey", "ScopeType", "ScopeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plants_Code",
                table: "Plants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreventionControls_FailureCauseId",
                table: "PreventionControls",
                column: "FailureCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSteps_FmeaRevisionId",
                table: "ProcessSteps",
                column: "FmeaRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSteps_FmeaRevisionId_Sequence",
                table: "ProcessSteps",
                columns: new[] { "FmeaRevisionId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSteps_ProductProcessStepId",
                table: "ProcessSteps",
                column: "ProductProcessStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLines_PlantId_Code",
                table: "ProductionLines",
                columns: new[] { "PlantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductProcesses_ProjectId",
                table: "ProductProcesses",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductProcessSteps_ProductProcessId_Sequence",
                table: "ProductProcessSteps",
                columns: new[] { "ProductProcessId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductionLineId_Code",
                table: "Products",
                columns: new[] { "ProductionLineId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAuditEvents_ActorUserKey",
                table: "ProjectAuditEvents",
                column: "ActorUserKey");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAuditEvents_ProjectId_OccurredAt",
                table: "ProjectAuditEvents",
                columns: new[] { "ProjectId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Code",
                table: "Projects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CustomerProfileId",
                table: "Projects",
                column: "CustomerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProductId",
                table: "Projects",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_FailureCauseId",
                table: "RiskAssessments",
                column: "FailureCauseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlPlanItems");

            migrationBuilder.DropTable(
                name: "DetectionControls");

            migrationBuilder.DropTable(
                name: "FailureEffects");

            migrationBuilder.DropTable(
                name: "PermissionAssignments");

            migrationBuilder.DropTable(
                name: "PreventionControls");

            migrationBuilder.DropTable(
                name: "ProjectAuditEvents");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "ControlPlans");

            migrationBuilder.DropTable(
                name: "FailureCauses");

            migrationBuilder.DropTable(
                name: "FailureModes");

            migrationBuilder.DropTable(
                name: "ProcessSteps");

            migrationBuilder.DropTable(
                name: "FmeaRevisions");

            migrationBuilder.DropTable(
                name: "ProductProcessSteps");

            migrationBuilder.DropTable(
                name: "Fmeas");

            migrationBuilder.DropTable(
                name: "ProductProcesses");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "CustomerProfiles");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductionLines");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
