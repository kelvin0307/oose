using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class removeMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_Lessons_LearningOutcomeId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_LearningOutcomes_LessonId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "CourseExecutionMaterial");

            migrationBuilder.DropTable(
                name: "CourseExecution");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LearningOutcomes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_LearningOutcomes_LessonId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLearningOutcome_Lessons_LearningOutcomeId",
                table: "LessonLearningOutcome");

            migrationBuilder.DropColumn(
                name: "EndQualification",
                table: "LearningOutcomes");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LearningOutcomes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "CourseExecution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseExecution_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseExecutionId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Class_CourseExecution_CourseExecutionId",
                        column: x => x.CourseExecutionId,
                        principalTable: "CourseExecution",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseExecutionMaterial",
                columns: table => new
                {
                    CourseExecutionsId = table.Column<int>(type: "int", nullable: false),
                    MaterialsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseExecutionMaterial", x => new { x.CourseExecutionsId, x.MaterialsId });
                    table.ForeignKey(
                        name: "FK_CourseExecutionMaterial_CourseExecution_CourseExecutionsId",
                        column: x => x.CourseExecutionsId,
                        principalTable: "CourseExecution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseExecutionMaterial_Materials_MaterialsId",
                        column: x => x.MaterialsId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Class_CourseExecutionId",
                table: "Class",
                column: "CourseExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseExecution_CourseId",
                table: "CourseExecution",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseExecutionMaterial_MaterialsId",
                table: "CourseExecutionMaterial",
                column: "MaterialsId");

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
