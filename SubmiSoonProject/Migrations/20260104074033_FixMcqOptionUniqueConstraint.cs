using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubmiSoonProject.Migrations
{
    /// <inheritdoc />
    public partial class FixMcqOptionUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_McqOptions_OptionLabel_OptionText",
                table: "McqOptions");

            migrationBuilder.DropIndex(
                name: "IX_McqOptions_QuestionId",
                table: "McqOptions");

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "McqOptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_McqOptions_QuestionId_OptionLabel",
                table: "McqOptions",
                columns: new[] { "QuestionId", "OptionLabel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_McqOptions_QuestionId_OptionLabel",
                table: "McqOptions");

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "McqOptions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_McqOptions_OptionLabel_OptionText",
                table: "McqOptions",
                columns: new[] { "OptionLabel", "OptionText" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_McqOptions_QuestionId",
                table: "McqOptions",
                column: "QuestionId");
        }
    }
}
