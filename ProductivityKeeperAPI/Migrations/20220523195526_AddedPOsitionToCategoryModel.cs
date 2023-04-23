using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class AddedPOsitionToCategoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Category");
        }
    }
}
