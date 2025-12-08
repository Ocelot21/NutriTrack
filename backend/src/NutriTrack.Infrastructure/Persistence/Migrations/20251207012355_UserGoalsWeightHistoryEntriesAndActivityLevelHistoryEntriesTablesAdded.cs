using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriTrack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserGoalsWeightHistoryEntriesAndActivityLevelHistoryEntriesTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLevelHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLevelHistoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLevelHistoryEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartWeightKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    TargetWeightKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeightHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightHistoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightHistoryEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLevelHistoryEntries_UserId",
                table: "ActivityLevelHistoryEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLevelHistoryEntries_UserId_EffectiveFrom",
                table: "ActivityLevelHistoryEntries",
                columns: new[] { "UserId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_UserGoals_UserId",
                table: "UserGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGoals_UserId_Status",
                table: "UserGoals",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WeightHistoryEntries_UserId",
                table: "WeightHistoryEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightHistoryEntries_UserId_Date",
                table: "WeightHistoryEntries",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLevelHistoryEntries");

            migrationBuilder.DropTable(
                name: "UserGoals");

            migrationBuilder.DropTable(
                name: "WeightHistoryEntries");
        }
    }
}
