using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class addFeedbackToGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Grades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GradeValue",
                table: "Grades",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "GradeValue",
                table: "Grades");
        }
    }
}
