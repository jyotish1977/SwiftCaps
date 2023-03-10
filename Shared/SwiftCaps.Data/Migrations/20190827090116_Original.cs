using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SwiftCaps.Data.Migrations
{
    public partial class Original : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizAnswer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    QuestionId = table.Column<Guid>(nullable: false),
                    ActualAnswer = table.Column<string>(nullable: true),
                    UserAnswer = table.Column<string>(nullable: true),
                    AnswerIndex = table.Column<int>(nullable: false),
                    AnswerPrefix = table.Column<string>(nullable: true),
                    AnswerSuffix = table.Column<string>(nullable: true),
                    AnswerLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    InfoMarkdown = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupQuizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: false),
                    Recurrence = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "QuizSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    QuizId = table.Column<Guid>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSections_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserQuizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    GroupQuizId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Expiry = table.Column<DateTime>(nullable: false),
                    Completed = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserQuizzes_GroupQuizzes_GroupQuizId",
                        column: x => x.GroupQuizId,
                        principalTable: "GroupQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    QuizSectionId = table.Column<Guid>(nullable: false),
                    QuizSectionIndex = table.Column<int>(nullable: false),
                    Header = table.Column<string>(nullable: true),
                    Footer = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_QuizSections_QuizSectionId",
                        column: x => x.QuizSectionId,
                        principalTable: "QuizSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupQuizzes_GroupId",
                table: "GroupQuizzes",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupQuizzes_QuizId",
                table: "GroupQuizzes",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizSectionId",
                table: "Questions",
                column: "QuizSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSections_QuizId",
                table: "QuizSections",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_GroupQuizId",
                table: "UserQuizzes",
                column: "GroupQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "QuizAnswer");

            migrationBuilder.DropTable(
                name: "UserQuizzes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "QuizSections");

            migrationBuilder.DropTable(
                name: "GroupQuizzes");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
