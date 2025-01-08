using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labb3_Anropa_databasen.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Courses__C92D7187092125E3", x => x.CourseID);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    TeacherID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__EDF259446484B98E", x => x.TeacherID);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EnrollmentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GraduationDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Students__32C52A7968C72E03", x => x.StudentID);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    EnrollmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID_FK = table.Column<int>(type: "int", nullable: true),
                    CourseID_FK = table.Column<int>(type: "int", nullable: true),
                    TeacherID_FK = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Enrollme__7F6877FB137C6997", x => x.EnrollmentID);
                    table.ForeignKey(
                        name: "FK__Enrollmen__Cours__2E1BDC42",
                        column: x => x.CourseID_FK,
                        principalTable: "Courses",
                        principalColumn: "CourseID");
                    table.ForeignKey(
                        name: "FK__Enrollmen__Stude__2D27B809",
                        column: x => x.StudentID_FK,
                        principalTable: "Students",
                        principalColumn: "StudentID");
                    table.ForeignKey(
                        name: "FK__Enrollmen__Teach__2F10007B",
                        column: x => x.TeacherID_FK,
                        principalTable: "Employees",
                        principalColumn: "TeacherID");
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    GradeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumericGrade = table.Column<int>(type: "int", nullable: true),
                    GradeSetDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EnrollmentID_FK = table.Column<int>(type: "int", nullable: true),
                    TeacherID_FK = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Grades__54F87A373D838EFB", x => x.GradeID);
                    table.ForeignKey(
                        name: "FK__Grades__Enrollme__34C8D9D1",
                        column: x => x.EnrollmentID_FK,
                        principalTable: "Enrollments",
                        principalColumn: "EnrollmentID");
                    table.ForeignKey(
                        name: "FK__Grades__TeacherI__35BCFE0A",
                        column: x => x.TeacherID_FK,
                        principalTable: "Employees",
                        principalColumn: "TeacherID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseID_FK",
                table: "Enrollments",
                column: "CourseID_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentID_FK",
                table: "Enrollments",
                column: "StudentID_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TeacherID_FK",
                table: "Enrollments",
                column: "TeacherID_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_EnrollmentID_FK",
                table: "Grades",
                column: "EnrollmentID_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TeacherID_FK",
                table: "Grades",
                column: "TeacherID_FK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
