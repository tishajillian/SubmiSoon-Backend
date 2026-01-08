using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubmiSoonProject.Migrations
{
    /// <inheritdoc />
    public partial class FixShadowPropertyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_UserAssessments_UserAssessmentId",
                table: "Answers");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_UserAssessments_UserAssessmentId",
                table: "Answers",
                column: "UserAssessmentId",
                principalTable: "UserAssessments",
                principalColumn: "UserAssessmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_UserAssessments_UserAssessmentId",
                table: "Answers");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_UserAssessments_UserAssessmentId",
                table: "Answers",
                column: "UserAssessmentId",
                principalTable: "UserAssessments",
                principalColumn: "UserAssessmentId");
        }
    }
}
