using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Temple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunicationLayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerThreads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LockedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerThreads_StudentAnswers_StudentAnswerId",
                        column: x => x.StudentAnswerId,
                        principalTable: "StudentAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherCheckQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionKey = table.Column<Guid>(type: "uuid", nullable: false),
                    NoteText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherCheckQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherCheckQuestions_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThreadMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyText = table.Column<string>(type: "text", nullable: false),
                    SourceCheckQuestionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreadMessages_AnswerThreads_AnswerThreadId",
                        column: x => x.AnswerThreadId,
                        principalTable: "AnswerThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadMessages_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerThreads_StudentAnswerId",
                table: "AnswerThreads",
                column: "StudentAnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCheckQuestions_TeacherId_QuestionKey",
                table: "TeacherCheckQuestions",
                columns: new[] { "TeacherId", "QuestionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreadMessages_AnswerThreadId",
                table: "ThreadMessages",
                column: "AnswerThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadMessages_AuthorId",
                table: "ThreadMessages",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherCheckQuestions");

            migrationBuilder.DropTable(
                name: "ThreadMessages");

            migrationBuilder.DropTable(
                name: "AnswerThreads");
        }
    }
}
