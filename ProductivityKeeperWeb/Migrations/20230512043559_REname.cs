using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductivityKeeperWeb.Migrations
{
    /// <inheritdoc />
    public partial class REname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubcaategoryId",
                table: "SubcategoryTask");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubcaategoryId",
                table: "SubcategoryTask",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
