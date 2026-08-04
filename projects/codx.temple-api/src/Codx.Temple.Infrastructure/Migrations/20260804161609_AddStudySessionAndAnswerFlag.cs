using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Temple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudySessionAndAnswerFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Reviewed",
                table: "StudentAnswers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewedAt",
                table: "StudentAnswers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedBy",
                table: "StudentAnswers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByUserId",
                table: "StudentAnswers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedInSessionId",
                table: "StudentAnswers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudySessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    StartQuestionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EndQuestionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentQuestionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySessions_LessonAttempts_LessonAttemptId",
                        column: x => x.LessonAttemptId,
                        principalTable: "LessonAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionKey = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlagType = table.Column<string>(type: "text", nullable: false),
                    RaisedInSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerFlags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerFlags_LessonAttempts_LessonAttemptId",
                        column: x => x.LessonAttemptId,
                        principalTable: "LessonAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerFlags_StudySessions_RaisedInSessionId",
                        column: x => x.RaisedInSessionId,
                        principalTable: "StudySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerFlags_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_ReviewedByUserId",
                table: "StudentAnswers",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_ReviewedInSessionId",
                table: "StudentAnswers",
                column: "ReviewedInSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerFlags_LessonAttemptId",
                table: "AnswerFlags",
                column: "LessonAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerFlags_RaisedInSessionId",
                table: "AnswerFlags",
                column: "RaisedInSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerFlags_StudentId_ResolvedAt",
                table: "AnswerFlags",
                columns: new[] { "StudentId", "ResolvedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_LessonAttemptId",
                table: "StudySessions",
                column: "LessonAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudySessions_ReviewedInSessionId",
                table: "StudentAnswers",
                column: "ReviewedInSessionId",
                principalTable: "StudySessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Users_ReviewedByUserId",
                table: "StudentAnswers",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudySessions_ReviewedInSessionId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Users_ReviewedByUserId",
                table: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "AnswerFlags");

            migrationBuilder.DropTable(
                name: "StudySessions");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_ReviewedByUserId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_ReviewedInSessionId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "ReviewedInSessionId",
                table: "StudentAnswers");
        }
    }
}
