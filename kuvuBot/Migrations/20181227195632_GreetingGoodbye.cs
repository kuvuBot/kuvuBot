using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class GreetingGoodbye : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Greeting",
                table: "Guilds",
                newName: "GreetingChannel");

            migrationBuilder.AddColumn<ulong>(
                name: "GoodbyeChannel",
                table: "Guilds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoodbyeMessage",
                table: "Guilds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GreetingMessage",
                table: "Guilds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoodbyeChannel",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "GoodbyeMessage",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "GreetingMessage",
                table: "Guilds");

            migrationBuilder.RenameColumn(
                name: "GreetingChannel",
                table: "Guilds",
                newName: "Greeting");
        }
    }
}
