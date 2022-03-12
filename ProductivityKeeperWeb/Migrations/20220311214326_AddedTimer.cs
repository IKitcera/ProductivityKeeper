using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class AddedTimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimerId",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Timer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticked = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Goal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_TimerId",
                table: "Units",
                column: "TimerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Timer_TimerId",
                table: "Units",
                column: "TimerId",
                principalTable: "Timer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Timer_TimerId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "Timer");

            migrationBuilder.DropIndex(
                name: "IX_Units_TimerId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "TimerId",
                table: "Units");
        }
    }
}
