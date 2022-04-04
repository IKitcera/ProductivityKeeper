using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class FixedMisssingOwnedAttribteForTaskToMAnySub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSubcategory_TaskToManySubcategories_TaskToManySubcategoriesId",
                table: "TaskSubcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskToManySubcategories_Units_UnitId",
                table: "TaskToManySubcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskToManySubcategories",
                table: "TaskToManySubcategories");

            migrationBuilder.DropIndex(
                name: "IX_TaskToManySubcategories_UnitId",
                table: "TaskToManySubcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskSubcategory",
                table: "TaskSubcategory");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "TaskToManySubcategories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskToManySubcategoriesUnitId",
                table: "TaskSubcategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskToManySubcategories",
                table: "TaskToManySubcategories",
                columns: new[] { "UnitId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskSubcategory",
                table: "TaskSubcategory",
                columns: new[] { "TaskToManySubcategoriesUnitId", "TaskToManySubcategoriesId", "Id" });

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

            migrationBuilder.DropColumn(
                name: "TaskToManySubcategoriesUnitId",
                table: "TaskSubcategory");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "TaskToManySubcategories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskToManySubcategories",
                table: "TaskToManySubcategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskSubcategory",
                table: "TaskSubcategory",
                columns: new[] { "TaskToManySubcategoriesId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskToManySubcategories_UnitId",
                table: "TaskToManySubcategories",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSubcategory_TaskToManySubcategories_TaskToManySubcategoriesId",
                table: "TaskSubcategory",
                column: "TaskToManySubcategoriesId",
                principalTable: "TaskToManySubcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskToManySubcategories_Units_UnitId",
                table: "TaskToManySubcategories",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
