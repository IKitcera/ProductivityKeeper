using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductivityKeeperWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedTask");

            migrationBuilder.CreateTable(
                name: "ArchivedTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DoneDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedTasks_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedTasks_UnitId",
                table: "ArchivedTasks",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedTasks");

            migrationBuilder.CreateTable(
                name: "ArchivedTask",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DoneDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
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
        }
    }
}
