using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class AddAssignmentVariants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfDuplicates",
                table: "Assignments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Assignments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssignmentVariant",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentId1 = table.Column<Guid>(nullable: false),
                    AssignmentId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    StudentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentVariant_Assignments_AssignmentId1",
                        column: x => x.AssignmentId1,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentVariant_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentVariant_AssignmentId1",
                table: "AssignmentVariant",
                column: "AssignmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentVariant_StudentId",
                table: "AssignmentVariant",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentVariant");

            migrationBuilder.DropColumn(
                name: "NumberOfDuplicates",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Assignments");
        }
    }
}
