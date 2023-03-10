using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SwiftCaps.Data.Migrations
{
    public partial class UpdateUserQuizWithSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_GroupQuizzes_GroupQuizId",
                table: "UserQuizzes");

            migrationBuilder.DropTable(
                name: "GroupQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_GroupQuizId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleGroups_ScheduleId",
                table: "ScheduleGroups");

            migrationBuilder.RenameColumn(
                name: "GroupQuizId",
                table: "UserQuizzes",
                newName: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_ScheduleId",
                table: "UserQuizzes",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleGroups_ScheduleId_GroupId",
                table: "ScheduleGroups",
                columns: new[] { "ScheduleId", "GroupId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_Schedules_ScheduleId",
                table: "UserQuizzes",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_Schedules_ScheduleId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_ScheduleId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleGroups_ScheduleId_GroupId",
                table: "ScheduleGroups");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "UserQuizzes",
                newName: "GroupQuizId");

            migrationBuilder.CreateTable(
                name: "GroupQuizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Recurrence = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupQuizzes_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupQuizzes_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_GroupQuizId",
                table: "UserQuizzes",
                column: "GroupQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleGroups_ScheduleId",
                table: "ScheduleGroups",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupQuizzes_GroupId",
                table: "GroupQuizzes",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupQuizzes_QuizId",
                table: "GroupQuizzes",
                column: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_GroupQuizzes_GroupQuizId",
                table: "UserQuizzes",
                column: "GroupQuizId",
                principalTable: "GroupQuizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
