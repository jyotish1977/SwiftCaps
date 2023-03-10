using Microsoft.EntityFrameworkCore.Migrations;

namespace SwiftCaps.Data.Migrations
{
    public partial class UpdateUserQuizScheduleRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_ScheduleId",
                table: "UserQuizzes");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_ScheduleId",
                table: "UserQuizzes",
                column: "ScheduleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_ScheduleId",
                table: "UserQuizzes");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_ScheduleId",
                table: "UserQuizzes",
                column: "ScheduleId",
                unique: true);
        }
    }
}
