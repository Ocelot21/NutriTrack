using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriTrack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<byte>(type: "tinyint", nullable: false),
                    DefaultCaloriesPerMinute = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groceries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProteinGramsPer100g = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CarbsGramsPer100g = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FatGramsPer100g = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CaloriesPer100 = table.Column<short>(type: "smallint", nullable: false),
                    UnitOfMeasure = table.Column<byte>(type: "tinyint", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groceries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    IsHealthProfileCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    ActivityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    Birthdate = table.Column<DateOnly>(type: "date", nullable: true),
                    HeightCm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OccurredAtLocal = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LocalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalCalories = table.Column<int>(type: "int", nullable: false),
                    TotalProtein = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    TotalCarbohydrates = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    TotalFats = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExerciseLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ExerciseCategory = table.Column<byte>(type: "tinyint", nullable: false),
                    CaloriesPerMinute = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    DurationMinutes = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OccurredAtLocal = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LocalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalCalories = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExerciseLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExerciseLogs_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserExerciseLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroceryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroceryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CaloriesPer100 = table.Column<short>(type: "smallint", nullable: false),
                    ProteinGramsPer100g = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CarbsGramsPer100g = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FatGramsPer100g = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    UnitOfMeasure = table.Column<byte>(type: "tinyint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealItem_Groceries_GroceryId",
                        column: x => x.GroceryId,
                        principalTable: "Groceries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealItem_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Category",
                table: "Exercises",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_IsApproved",
                table: "Exercises",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Name",
                table: "Exercises",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Groceries_Barcode",
                table: "Groceries",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Groceries_Category",
                table: "Groceries",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Groceries_Name",
                table: "Groceries",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MealItem_GroceryId",
                table: "MealItem",
                column: "GroceryId");

            migrationBuilder.CreateIndex(
                name: "IX_MealItem_MealId",
                table: "MealItem",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealItem_Quantity",
                table: "MealItem",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_LocalDate",
                table: "Meals",
                column: "LocalDate");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_OccurredAtUtc",
                table: "Meals",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_UserId",
                table: "Meals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Key",
                table: "Permissions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsActive",
                table: "Roles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseLogs_ExerciseId",
                table: "UserExerciseLogs",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseLogs_LocalDate",
                table: "UserExerciseLogs",
                column: "LocalDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseLogs_OccurredAtUtc",
                table: "UserExerciseLogs",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseLogs_UserId",
                table: "UserExerciseLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealItem");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserExerciseLogs");

            migrationBuilder.DropTable(
                name: "Groceries");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
