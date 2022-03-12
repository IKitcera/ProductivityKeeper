using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class FixedMisseOwnedAttributeToTheTimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Timer_TimerId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "Timer");

            migrationBuilder.DropIndex(
                name: "IX_Units_TimerId",
                table: "Units");

            migrationBuilder.RenameColumn(
                name: "TimerId",
                table: "Units",
                newName: "Timer_Id");

            migrationBuilder.AddColumn<long>(
                name: "Timer_Goal",
                table: "Units",
                type: "bigint",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timer_Goal",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Timer_Label",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Timer_Ticked",
                table: "Units");

            migrationBuilder.RenameColumn(
                name: "Timer_Id",
                table: "Units",
                newName: "TimerId");

            migrationBuilder.CreateTable(
                name: "Timer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Goal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticked = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
    }
}
