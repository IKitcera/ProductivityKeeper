using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class TasksRelatedPartivallyDeletedInTaskObj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_TaskToManySubcategories_Units_UnitId",
                table: "Units_TaskToManySubcategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_TaskToManySubcategories_TaskSubcategories_Units_TaskToManySubcategories_TaskToManySubcategoriesUnitId_TaskToManySubcat~",
                table: "Units_TaskToManySubcategories_TaskSubcategories");

            migrationBuilder.DropTable(
                name: "Task_TaskSubcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Units_TaskToManySubcategories_TaskSubcategories",
                table: "Units_TaskToManySubcategories_TaskSubcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Units_TaskToManySubcategories",
                table: "Units_TaskToManySubcategories");

            migrationBuilder.DropColumn(
                name: "RelatedTasks_Id",
                table: "Task");

            migrationBuilder.RenameTable(
                name: "Units_TaskToManySubcategories_TaskSubcategories",
                newName: "TaskSubcategory");

            migrationBuilder.RenameTable(
                name: "Units_TaskToManySubcategories",
                newName: "TaskToManySubcategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskSubcategory",
                table: "TaskSubcategory",
                columns: new[] { "TaskToManySubcategoriesUnitId", "TaskToManySubcategoriesId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskToManySubcategories",
                table: "TaskToManySubcategories",
                columns: new[] { "UnitId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSubcategory_TaskToManySubcategories_TaskToManySubcategoriesUnitId_TaskToManySubcategoriesId",
                table: "TaskSubcategory",
                columns: new[] { "TaskToManySubcategoriesUnitId", "TaskToManySubcategoriesId" },
                principalTable: "TaskToManySubcategories",
                principalColumns: new[] { "UnitId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskToManySubcategories_Units_UnitId",
                table: "TaskToManySubcategories",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSubcategory_TaskToManySubcategories_TaskToManySubcategoriesUnitId_TaskToManySubcategoriesId",
                table: "TaskSubcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskToManySubcategories_Units_UnitId",
                table: "TaskToManySubcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskToManySubcategories",
                table: "TaskToManySubcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskSubcategory",
                table: "TaskSubcategory");

            migrationBuilder.RenameTable(
                name: "TaskToManySubcategories",
                newName: "Units_TaskToManySubcategories");

            migrationBuilder.RenameTable(
                name: "TaskSubcategory",
                newName: "Units_TaskToManySubcategories_TaskSubcategories");

            migrationBuilder.AddColumn<int>(
                name: "RelatedTasks_Id",
                table: "Task",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units_TaskToManySubcategories",
                table: "Units_TaskToManySubcategories",
                columns: new[] { "UnitId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units_TaskToManySubcategories_TaskSubcategories",
                table: "Units_TaskToManySubcategories_TaskSubcategories",
                columns: new[] { "TaskToManySubcategoriesUnitId", "TaskToManySubcategoriesId", "Id" });

            migrationBuilder.CreateTable(
                name: "Task_TaskSubcategories",
                columns: table => new
                {
                    TaskToManySubcategoriesTaskSubcategoryCategoryUnitId = table.Column<int>(type: "int", nullable: false),
                    TaskToManySubcategoriesTaskSubcategoryCategoryId = table.Column<int>(type: "int", nullable: false),
                    TaskToManySubcategoriesTaskSubcategoryId = table.Column<int>(type: "int", nullable: false),
                    TaskToManySubcategoriesTaskId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task_TaskSubcategories", x => new { x.TaskToManySubcategoriesTaskSubcategoryCategoryUnitId, x.TaskToManySubcategoriesTaskSubcategoryCategoryId, x.TaskToManySubcategoriesTaskSubcategoryId, x.TaskToManySubcategoriesTaskId, x.Id });
                    table.ForeignKey(
                        name: "FK_Task_TaskSubcategories_Task_TaskToManySubcategoriesTaskSubcategoryCategoryUnitId_TaskToManySubcategoriesTaskSubcategoryCateg~",
                        columns: x => new { x.TaskToManySubcategoriesTaskSubcategoryCategoryUnitId, x.TaskToManySubcategoriesTaskSubcategoryCategoryId, x.TaskToManySubcategoriesTaskSubcategoryId, x.TaskToManySubcategoriesTaskId },
                        principalTable: "Task",
                        principalColumns: new[] { "SubcategoryCategoryUnitId", "SubcategoryCategoryId", "SubcategoryId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Units_TaskToManySubcategories_Units_UnitId",
                table: "Units_TaskToManySubcategories",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_TaskToManySubcategories_TaskSubcategories_Units_TaskToManySubcategories_TaskToManySubcategoriesUnitId_TaskToManySubcat~",
                table: "Units_TaskToManySubcategories_TaskSubcategories",
                columns: new[] { "TaskToManySubcategoriesUnitId", "TaskToManySubcategoriesId" },
                principalTable: "Units_TaskToManySubcategories",
                principalColumns: new[] { "UnitId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
