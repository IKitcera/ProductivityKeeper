using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductivityKeeperWeb.Migrations
{
    /// <inheritdoc />
    public partial class Refactoreddoneperdaydata2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonePerDays_Statistics_StatisticId",
                table: "DonePerDays");

            migrationBuilder.AlterColumn<int>(
                name: "StatisticId",
                table: "DonePerDays",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DonePerDays_Statistics_StatisticId",
                table: "DonePerDays",
                column: "StatisticId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonePerDays_Statistics_StatisticId",
                table: "DonePerDays");

            migrationBuilder.AlterColumn<int>(
                name: "StatisticId",
                table: "DonePerDays",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DonePerDays_Statistics_StatisticId",
                table: "DonePerDays",
                column: "StatisticId",
                principalTable: "Statistics",
                principalColumn: "Id");
        }
    }
}
