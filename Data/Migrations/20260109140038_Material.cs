using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Material : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Plannings_PlanningId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Plannings_Courses_CourseId",
                table: "Plannings");

            migrationBuilder.DropIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings");

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

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LearningOutcomes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings",
                column: "CourseId",
                unique: true,
                filter: "[CourseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_LessonId",
                table: "Materials",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

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
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Plannings_PlanningId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Plannings_Courses_CourseId",
                table: "Plannings");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings");

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

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LearningOutcomes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_CourseId",
                table: "Plannings",
                column: "CourseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
