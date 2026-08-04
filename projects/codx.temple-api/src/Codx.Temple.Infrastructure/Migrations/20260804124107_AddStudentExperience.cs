using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Temple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonKey = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonAttempts_LessonVersions_LessonVersionId",
                        column: x => x.LessonVersionId,
                        principalTable: "LessonVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonAttempts_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentQuestionNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionKey = table.Column<Guid>(type: "uuid", nullable: false),
                    NoteText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuestionNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentQuestionNotes_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionKey = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerValue = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    PromptSnapshot = table.Column<string>(type: "text", nullable: false),
                    QuestionTypeSnapshot = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_LessonAttempts_LessonAttemptId",
                        column: x => x.LessonAttemptId,
                        principalTable: "LessonAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttempts_LessonVersionId",
                table: "LessonAttempts",
                column: "LessonVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttempts_StudentId_LessonKey",
                table: "LessonAttempts",
                columns: new[] { "StudentId", "LessonKey" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_LessonAttemptId_QuestionKey",
                table: "StudentAnswers",
                columns: new[] { "LessonAttemptId", "QuestionKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentId",
                table: "StudentAnswers",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuestionNotes_StudentId_QuestionKey",
                table: "StudentQuestionNotes",
                columns: new[] { "StudentId", "QuestionKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "StudentQuestionNotes");

            migrationBuilder.DropTable(
                name: "LessonAttempts");
        }
    }
}
