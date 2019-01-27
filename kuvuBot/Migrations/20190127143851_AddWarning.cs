using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class AddWarning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Warning",
                table: "Warns",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warns_Users_UserId",
                table: "Warns");

            migrationBuilder.DropIndex(
                name: "IX_Warns_UserId",
                table: "Warns");

            migrationBuilder.DropColumn(
                name: "Warning",
                table: "Warns");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Warns",
                newName: "TagWarningUserId");

            migrationBuilder.AddColumn<int>(
                name: "KuvuUserId",
                table: "Warns",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagUserId",
                table: "Warns",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KuvuWarnTag",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    WarningUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KuvuWarnTag", x => new { x.UserId, x.WarningUserId });
                    table.ForeignKey(
                        name: "FK_KuvuWarnTag_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KuvuWarnTag_Users_WarningUserId",
                        column: x => x.WarningUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warns_KuvuUserId",
                table: "Warns",
                column: "KuvuUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Warns_TagUserId_TagWarningUserId",
                table: "Warns",
                columns: new[] { "TagUserId", "TagWarningUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_KuvuWarnTag_WarningUserId",
                table: "KuvuWarnTag",
                column: "WarningUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Warns_Users_KuvuUserId",
                table: "Warns",
                column: "KuvuUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warns_KuvuWarnTag_TagUserId_TagWarningUserId",
                table: "Warns",
                columns: new[] { "TagUserId", "TagWarningUserId" },
                principalTable: "KuvuWarnTag",
                principalColumns: new[] { "UserId", "WarningUserId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
