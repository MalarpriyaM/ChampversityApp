using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Champversity.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighestQualification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredUniversity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredCourse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsProfileComplete = table.Column<bool>(type: "bit", nullable: false),
                    UniversityFileReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationSubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewSlots_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewSlots_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSlots_StudentId",
                table: "InterviewSlots",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSlots_StudentId1",
                table: "InterviewSlots",
                column: "StudentId1",
                unique: true,
                filter: "[StudentId1] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewSlots");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
