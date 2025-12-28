using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class removedMinimumLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_LearningOutcomes_LearningOutcomeId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_Lessons_LessonId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Plannings_PlanningId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Plannings_Courses_CourseId",
                table: "Plannings");

            migrationBuilder.DropIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings");

            migrationBuilder.DropColumn(
                name: "MinimumLessons",
                table: "LearningOutcomes");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Plannings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PlanningId",
                table: "Lessons",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "EndQualification",
                table: "LearningOutcomes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlanningId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings",
                column: "CourseId",
                unique: true,
                filter: "[CourseId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonLearningOutcome_LearningOutcomes_LessonId",
                table: "LessonLearningOutcome",
                column: "LessonId",
                principalTable: "LearningOutcomes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonLearningOutcome_Lessons_LearningOutcomeId",
                table: "LessonLearningOutcome",
                column: "LearningOutcomeId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Plannings_PlanningId",
                table: "Lessons",
                column: "PlanningId",
                principalTable: "Plannings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plannings_Courses_CourseId",
                table: "Plannings",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_LearningOutcomes_LessonId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_Lessons_LearningOutcomeId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Plannings_PlanningId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Plannings_Courses_CourseId",
                table: "Plannings");

            migrationBuilder.DropIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings");

            migrationBuilder.DropColumn(
                name: "EndQualification",
                table: "LearningOutcomes");

            migrationBuilder.DropColumn(
                name: "PlanningId",
                table: "Courses");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Plannings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanningId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimumLessons",
                table: "LearningOutcomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings",
                column: "CourseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonLearningOutcome_LearningOutcomes_LearningOutcomeId",
                table: "LessonLearningOutcome",
                column: "LearningOutcomeId",
                principalTable: "LearningOutcomes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonLearningOutcome_Lessons_LessonId",
                table: "LessonLearningOutcome",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Plannings_PlanningId",
                table: "Lessons",
                column: "PlanningId",
                principalTable: "Plannings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plannings_Courses_CourseId",
                table: "Plannings",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
