using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class RefactorChannelsIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GreetingChannel",
                table: "Guilds",
                newName: "GreetingChannelId");

            migrationBuilder.RenameColumn(
                name: "GoodbyeChannel",
                table: "Guilds",
                newName: "GoodbyeChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GreetingChannelId",
                table: "Guilds",
                newName: "GreetingChannel");

            migrationBuilder.RenameColumn(
                name: "GoodbyeChannelId",
                table: "Guilds",
                newName: "GoodbyeChannel");
        }
    }
}
