using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class AddLastExpMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastExpMessage",
                table: "Users",
                nullable: true,
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastExpMessage",
                table: "Users");
        }
    }
}
