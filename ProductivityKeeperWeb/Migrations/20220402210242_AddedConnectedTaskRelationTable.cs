using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class AddedConnectedTaskRelationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskToManySubcategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskToManySubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskToManySubcategories_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskSubcategory",
                columns: table => new
                {
                    TaskToManySubcategoriesId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSubcategory", x => new { x.TaskToManySubcategoriesId, x.Id });
                    table.ForeignKey(
                        name: "FK_TaskSubcategory_TaskToManySubcategories_TaskToManySubcategoriesId",
                        column: x => x.TaskToManySubcategoriesId,
                        principalTable: "TaskToManySubcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskToManySubcategories_UnitId",
                table: "TaskToManySubcategories",
                column: "UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskSubcategory");

            migrationBuilder.DropTable(
                name: "TaskToManySubcategories");
        }
    }
}
