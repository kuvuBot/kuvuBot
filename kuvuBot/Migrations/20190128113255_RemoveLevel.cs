using Microsoft.EntityFrameworkCore.Migrations;

namespace kuvuBot.Migrations
{
    public partial class RemoveLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "Exp",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "Exp",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<uint>(
                name: "Level",
                table: "Users",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
