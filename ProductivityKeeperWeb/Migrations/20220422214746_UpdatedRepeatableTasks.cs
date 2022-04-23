using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityKeeperWeb.Migrations
{
    public partial class UpdatedRepeatableTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HabbitInterval",
                table: "Task");

            migrationBuilder.AddColumn<double>(
                name: "HabbitIntervalInHours",
                table: "Task",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HabbitIntervalInHours",
                table: "Task");

            migrationBuilder.AddColumn<DateTime>(
                name: "HabbitInterval",
                table: "Task",
                type: "datetime2",
                nullable: true);
        }
    }
}
