using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorQuiz.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsApproved", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAENO/a7efm+SmpCoZgdAHBYdyYe5usyHgS8eXG/OPCuxuHdnmgZj9/O5V59AxLTr6uQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFtdXiFt2oP/r/R8Uc+pWL8ZOGSnK1Ua70J5mLHq2q95YLEi7MUo1Kik8RXTN+HC6A==");
        }
    }
}
