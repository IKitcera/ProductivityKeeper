using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class AddedStatissticToUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Statistic_CountOfDoneToday",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Statistic_CountOfDoneTotal",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Statistic_CountOfExpiredTotal",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Statistic_Id",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Statistic_PercentOfDoneToday",
                table: "Units",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Statistic_PercentOfDoneTotal",
                table: "Units",
                type: "real",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DonePerDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserStatisticUnitId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountOfDone = table.Column<float>(type: "real", nullable: false)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonePerDay");

            migrationBuilder.DropColumn(
                name: "Statistic_CountOfDoneToday",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Statistic_CountOfDoneTotal",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Statistic_CountOfExpiredTotal",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Statistic_Id",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Statistic_PercentOfDoneToday",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Statistic_PercentOfDoneTotal",
                table: "Units");
        }
    }
}
