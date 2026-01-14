using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ReaddMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseExecution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => new { x.Id, x.Version });
                    table.ForeignKey(
                        name: "FK_Materials_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseExecutionId = table.Column<int>(type: "int", nullable: true)
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
                    MaterialsId = table.Column<int>(type: "int", nullable: false),
                    MaterialsVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseExecutionMaterial", x => new { x.CourseExecutionsId, x.MaterialsId, x.MaterialsVersion });
                    table.ForeignKey(
                        name: "FK_CourseExecutionMaterial_CourseExecution_CourseExecutionsId",
                        column: x => x.CourseExecutionsId,
                        principalTable: "CourseExecution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseExecutionMaterial_Materials_MaterialsId_MaterialsVersion",
                        columns: x => new { x.MaterialsId, x.MaterialsVersion },
                        principalTable: "Materials",
                        principalColumns: new[] { "Id", "Version" },
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
                name: "IX_CourseExecutionMaterial_MaterialsId_MaterialsVersion",
                table: "CourseExecutionMaterial",
                columns: new[] { "MaterialsId", "MaterialsVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_LessonId",
                table: "Materials",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "CourseExecutionMaterial");

            migrationBuilder.DropTable(
                name: "CourseExecution");

            migrationBuilder.DropTable(
                name: "Materials");
        }
    }
}
