using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class MakeKuvuUserPerGuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warns_Guilds_GuildId",
                table: "Warns");

            migrationBuilder.DropIndex(
                name: "IX_Warns_GuildId",
                table: "Warns");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Warns");

            migrationBuilder.AddColumn<int>(
                name: "GuildId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GuildId",
                table: "Users",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Guilds_GuildId",
                table: "Users",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Guilds_GuildId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GuildId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "GuildId",
                table: "Warns",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warns_GuildId",
                table: "Warns",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Warns_Guilds_GuildId",
                table: "Warns",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
