using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class GlobalUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalRank",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "GlobalUserId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GlobalUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordUser = table.Column<ulong>(nullable: false),
                    GlobalRank = table.Column<string>(nullable: true),
                    Reputation = table.Column<int>(nullable: false),
                    Money = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GlobalUserId",
                table: "Users",
                column: "GlobalUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_GlobalUsers_GlobalUserId",
                table: "Users",
                column: "GlobalUserId",
                principalTable: "GlobalUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_GlobalUsers_GlobalUserId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "GlobalUsers");

            migrationBuilder.DropIndex(
                name: "IX_Users_GlobalUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GlobalUserId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "GlobalRank",
                table: "Users",
                nullable: true);
        }
    }
}
