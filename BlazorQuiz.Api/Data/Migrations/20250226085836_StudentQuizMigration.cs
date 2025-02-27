using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorQuiz.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class StudentQuizMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "StudentQuizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StudentQuestions",
                columns: table => new
                {
                    StudentQuizId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuestions", x => new { x.StudentQuizId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_StudentQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentQuestions_StudentQuizzes_StudentQuizId",
                        column: x => x.StudentQuizId,
                        principalTable: "StudentQuizzes",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAavaU4sOaKqxNeq6eC/M7gRanvFLOaIyybLCUkRb5v9wwXKEAZoI+eYHyq2/dX7Ew==");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuestions_QuestionId",
                table: "StudentQuestions",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentQuestions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentQuizzes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENO/a7efm+SmpCoZgdAHBYdyYe5usyHgS8eXG/OPCuxuHdnmgZj9/O5V59AxLTr6uQ==");
        }
    }
}
