using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductivityKeeperWeb.Migrations
{
    /// <inheritdoc />
    public partial class Refactoreddoneperdaydata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonePerDay");

            migrationBuilder.CreateTable(
                name: "DonePerDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountOfDone = table.Column<int>(type: "int", nullable: false),
                    StatisticId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonePerDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonePerDays_Statistics_StatisticId",
                        column: x => x.StatisticId,
                        principalTable: "Statistics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonePerDays_StatisticId",
                table: "DonePerDays",
                column: "StatisticId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonePerDays");

            migrationBuilder.CreateTable(
                name: "DonePerDay",
                columns: table => new
                {
                    UserStatisticId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountOfDone = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonePerDay", x => new { x.UserStatisticId, x.Id });
                    table.ForeignKey(
                        name: "FK_DonePerDay_Statistics_UserStatisticId",
                        column: x => x.UserStatisticId,
                        principalTable: "Statistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
