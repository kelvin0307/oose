using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class finalizeDB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grade_CourseExecution_CourseExecutionId",
                table: "Grade");

            migrationBuilder.DropForeignKey(
                name: "FK_Grade_Lessons_LessonId",
                table: "Grade");

            migrationBuilder.DropForeignKey(
                name: "FK_Grade_Students_StudentId",
                table: "Grade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grade",
                table: "Grade");

            migrationBuilder.RenameTable(
                name: "Grade",
                newName: "Grades");

            migrationBuilder.RenameIndex(
                name: "IX_Grade_StudentId",
                table: "Grades",
                newName: "IX_Grades_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Grade_LessonId",
                table: "Grades",
                newName: "IX_Grades_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_Grade_CourseExecutionId",
                table: "Grades",
                newName: "IX_Grades_CourseExecutionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grades",
                table: "Grades",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_CourseExecution_CourseExecutionId",
                table: "Grades",
                column: "CourseExecutionId",
                principalTable: "CourseExecution",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Lessons_LessonId",
                table: "Grades",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Students_StudentId",
                table: "Grades",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_CourseExecution_CourseExecutionId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Lessons_LessonId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Students_StudentId",
                table: "Grades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grades",
                table: "Grades");

            migrationBuilder.RenameTable(
                name: "Grades",
                newName: "Grade");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_StudentId",
                table: "Grade",
                newName: "IX_Grade_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_LessonId",
                table: "Grade",
                newName: "IX_Grade_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_CourseExecutionId",
                table: "Grade",
                newName: "IX_Grade_CourseExecutionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grade",
                table: "Grade",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grade_CourseExecution_CourseExecutionId",
                table: "Grade",
                column: "CourseExecutionId",
                principalTable: "CourseExecution",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grade_Lessons_LessonId",
                table: "Grade",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Grade_Students_StudentId",
                table: "Grade",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
