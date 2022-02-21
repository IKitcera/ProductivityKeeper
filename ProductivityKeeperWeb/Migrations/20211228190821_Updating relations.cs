using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class Updatingrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Color_ColorId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_Units_UnitId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Subcategory_Category_CategoryId",
                table: "Subcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Subcategory_Color_ColorId",
                table: "Subcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Subcategory_SubcategoryId",
                table: "Task");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_SubcategoryId",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subcategory",
                table: "Subcategory");

            migrationBuilder.DropIndex(
                name: "IX_Subcategory_CategoryId",
                table: "Subcategory");

            migrationBuilder.DropIndex(
                name: "IX_Subcategory_ColorId",
                table: "Subcategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_ColorId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_UnitId",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "ColorId",
                table: "Subcategory",
                newName: "Color_R");

            migrationBuilder.RenameColumn(
                name: "ColorId",
                table: "Category",
                newName: "Color_R");

            migrationBuilder.AlterColumn<int>(
                name: "SubcategoryId",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubcategoryCategoryUnitId",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubcategoryCategoryId",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Subcategory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryUnitId",
                table: "Subcategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Color_A",
                table: "Subcategory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color_B",
                table: "Subcategory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color_G",
                table: "Subcategory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color_Id",
                table: "Subcategory",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color_A",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color_B",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color_G",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color_Id",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                table: "Task",
                columns: new[] { "SubcategoryCategoryUnitId", "SubcategoryCategoryId", "SubcategoryId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subcategory",
                table: "Subcategory",
                columns: new[] { "CategoryUnitId", "CategoryId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                columns: new[] { "UnitId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Units_UnitId",
                table: "Category",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subcategory_Category_CategoryUnitId_CategoryId",
                table: "Subcategory",
                columns: new[] { "CategoryUnitId", "CategoryId" },
                principalTable: "Category",
                principalColumns: new[] { "UnitId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Subcategory_SubcategoryCategoryUnitId_SubcategoryCategoryId_SubcategoryId",
                table: "Task",
                columns: new[] { "SubcategoryCategoryUnitId", "SubcategoryCategoryId", "SubcategoryId" },
                principalTable: "Subcategory",
                principalColumns: new[] { "CategoryUnitId", "CategoryId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Units_UnitId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Subcategory_Category_CategoryUnitId_CategoryId",
                table: "Subcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Subcategory_SubcategoryCategoryUnitId_SubcategoryCategoryId_SubcategoryId",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subcategory",
                table: "Subcategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "SubcategoryCategoryUnitId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "SubcategoryCategoryId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CategoryUnitId",
                table: "Subcategory");

            migrationBuilder.DropColumn(
                name: "Color_A",
                table: "Subcategory");

            migrationBuilder.DropColumn(
                name: "Color_B",
                table: "Subcategory");

            migrationBuilder.DropColumn(
                name: "Color_G",
                table: "Subcategory");

            migrationBuilder.DropColumn(
                name: "Color_Id",
                table: "Subcategory");

            migrationBuilder.DropColumn(
                name: "Color_A",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Color_B",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Color_G",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Color_Id",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "Color_R",
                table: "Subcategory",
                newName: "ColorId");

            migrationBuilder.RenameColumn(
                name: "Color_R",
                table: "Category",
                newName: "ColorId");

            migrationBuilder.AlterColumn<int>(
                name: "SubcategoryId",
                table: "Task",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Subcategory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Category",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                table: "Task",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subcategory",
                table: "Subcategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    A = table.Column<int>(type: "int", nullable: false),
                    B = table.Column<int>(type: "int", nullable: false),
                    G = table.Column<int>(type: "int", nullable: false),
                    R = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Color", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Task_SubcategoryId",
                table: "Task",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_CategoryId",
                table: "Subcategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_ColorId",
                table: "Subcategory",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_ColorId",
                table: "Category",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_UnitId",
                table: "Category",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Color_ColorId",
                table: "Category",
                column: "ColorId",
                principalTable: "Color",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Units_UnitId",
                table: "Category",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subcategory_Category_CategoryId",
                table: "Subcategory",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subcategory_Color_ColorId",
                table: "Subcategory",
                column: "ColorId",
                principalTable: "Color",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Subcategory_SubcategoryId",
                table: "Task",
                column: "SubcategoryId",
                principalTable: "Subcategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
