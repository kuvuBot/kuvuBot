using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class AddMuteRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "MuteRole",
                table: "Guilds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MuteRole",
                table: "Guilds");
        }
    }
}
