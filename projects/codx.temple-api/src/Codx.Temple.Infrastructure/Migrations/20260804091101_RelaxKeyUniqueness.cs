using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Temple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RelaxKeyUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_Key",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_LessonNodes_Key",
                table: "LessonNodes");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Key",
                table: "Questions",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNodes_Key",
                table: "LessonNodes",
                column: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_Key",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_LessonNodes_Key",
                table: "LessonNodes");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Key",
                table: "Questions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonNodes_Key",
                table: "LessonNodes",
                column: "Key",
                unique: true);
        }
    }
}
