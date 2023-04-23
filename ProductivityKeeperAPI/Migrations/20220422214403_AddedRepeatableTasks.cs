using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class AddedRepeatableTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoalRepeatCount",
                table: "Task",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HabbitInterval",
                table: "Task",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeatable",
                table: "Task",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TimesToRepeat",
                table: "Task",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalRepeatCount",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "HabbitInterval",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "IsRepeatable",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "TimesToRepeat",
                table: "Task");
        }
    }
}
