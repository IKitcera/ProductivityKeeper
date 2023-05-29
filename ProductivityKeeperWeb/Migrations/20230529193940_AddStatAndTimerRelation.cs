using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductivityKeeperWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddStatAndTimerRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timer_Format",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Timer_Goal",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Timer_Id",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Timer_Label",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Timer_Ticked",
                table: "Units");

            migrationBuilder.AddColumn<int>(
                name: "TimerId",
                table: "Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Timers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticked = table.Column<long>(type: "bigint", nullable: false),
                    Goal = table.Column<long>(type: "bigint", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timers_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_UnitId",
                table: "Statistics",
                column: "UnitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timers_UnitId",
                table: "Timers",
                column: "UnitId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Units_UnitId",
                table: "Statistics",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Units_UnitId",
                table: "Statistics");

            migrationBuilder.DropTable(
                name: "Timers");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_UnitId",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "TimerId",
                table: "Units");

            migrationBuilder.AddColumn<int>(
                name: "Timer_Format",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Timer_Goal",
                table: "Units",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Timer_Id",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timer_Label",
                table: "Units",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Timer_Ticked",
                table: "Units",
                type: "bigint",
                nullable: true);
        }
    }
}
