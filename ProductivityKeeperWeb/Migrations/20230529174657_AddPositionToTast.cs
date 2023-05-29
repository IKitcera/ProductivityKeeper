using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductivityKeeperWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToTast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Tasks");
        }
    }
}
