using Microsoft.EntityFrameworkCore.Migrations;

namespace SwiftCaps.Data.Migrations
{
    public partial class sequnce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "UserQuizzes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "UserQuizzes");
        }
    }
}
