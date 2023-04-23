using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timer_Id = table.Column<int>(type: "int", nullable: true),
                    Timer_Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timer_Ticked = table.Column<long>(type: "bigint", nullable: true),
                    Timer_Goal = table.Column<long>(type: "bigint", nullable: true),
                    Statistic_Id = table.Column<int>(type: "int", nullable: true),
                    Statistic_PercentOfDoneToday = table.Column<float>(type: "real", nullable: true),
                    Statistic_PercentOfDoneTotal = table.Column<float>(type: "real", nullable: true),
                    Statistic_CountOfDoneToday = table.Column<int>(type: "int", nullable: true),
                    Statistic_CountOfDoneTotal = table.Column<int>(type: "int", nullable: true),
                    Statistic_CountOfExpiredTotal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneratingColorsWay = table.Column<int>(type: "int", nullable: false),
                    AppTheme = table.Column<int>(type: "int", nullable: false),
                    IsSynchronizationOn = table.Column<bool>(type: "bit", nullable: false),
                    AutoMoveTasksThatExpired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArchivedTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DoneDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedTask", x => new { x.UnitId, x.Id });
                    table.ForeignKey(
                        name: "FK_ArchivedTask_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color_Id = table.Column<int>(type: "int", nullable: true),
                    Color_R = table.Column<int>(type: "int", nullable: true),
                    Color_G = table.Column<int>(type: "int", nullable: true),
                    Color_B = table.Column<int>(type: "int", nullable: true),
                    Color_A = table.Column<int>(type: "int", nullable: true),
                    DateOfCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => new { x.UnitId, x.Id });
                    table.ForeignKey(
                        name: "FK_Category_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonePerDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserStatisticUnitId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountOfDone = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonePerDay", x => new { x.UserStatisticUnitId, x.Id });
                    table.ForeignKey(
                        name: "FK_DonePerDay_Units_UserStatisticUnitId",
                        column: x => x.UserStatisticUnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskToManySubcategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskToManySubcategories", x => new { x.UnitId, x.Id });
                    table.ForeignKey(
                        name: "FK_TaskToManySubcategories_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UserSettingsId = table.Column<int>(type: "int", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HashPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                    table.ForeignKey(
                        name: "FK_Users_UserSettings_UserSettingsId",
                        column: x => x.UserSettingsId,
                        principalTable: "UserSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subcategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryUnitId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color_Id = table.Column<int>(type: "int", nullable: true),
                    Color_R = table.Column<int>(type: "int", nullable: true),
                    Color_G = table.Column<int>(type: "int", nullable: true),
                    Color_B = table.Column<int>(type: "int", nullable: true),
                    Color_A = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: false),
                    DateOfCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategory", x => new { x.CategoryUnitId, x.CategoryId, x.Id });
                    table.ForeignKey(
                        name: "FK_Subcategory_Category_CategoryUnitId_CategoryId",
                        columns: x => new { x.CategoryUnitId, x.CategoryId },
                        principalTable: "Category",
                        principalColumns: new[] { "UnitId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskSubcategory",
                columns: table => new
                {
                    TaskToManySubcategoriesUnitId = table.Column<int>(type: "int", nullable: false),
                    TaskToManySubcategoriesId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSubcategory", x => new { x.TaskToManySubcategoriesUnitId, x.TaskToManySubcategoriesId, x.Id });
                    table.ForeignKey(
                        name: "FK_TaskSubcategory_TaskToManySubcategories_TaskToManySubcategoriesUnitId_TaskToManySubcategoriesId",
                        columns: x => new { x.TaskToManySubcategoriesUnitId, x.TaskToManySubcategoriesId },
                        principalTable: "TaskToManySubcategories",
                        principalColumns: new[] { "UnitId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubcategoryCategoryUnitId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryCategoryId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChecked = table.Column<bool>(type: "bit", nullable: false),
                    DateOfCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DoneDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => new { x.SubcategoryCategoryUnitId, x.SubcategoryCategoryId, x.SubcategoryId, x.Id });
                    table.ForeignKey(
                        name: "FK_Task_Subcategory_SubcategoryCategoryUnitId_SubcategoryCategoryId_SubcategoryId",
                        columns: x => new { x.SubcategoryCategoryUnitId, x.SubcategoryCategoryId, x.SubcategoryId },
                        principalTable: "Subcategory",
                        principalColumns: new[] { "CategoryUnitId", "CategoryId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserSettingsId",
                table: "Users",
                column: "UserSettingsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedTask");

            migrationBuilder.DropTable(
                name: "DonePerDay");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "TaskSubcategory");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Subcategory");

            migrationBuilder.DropTable(
                name: "TaskToManySubcategories");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}
