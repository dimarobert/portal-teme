using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class FixAssignVariant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentVariant_Assignments_AssignmentId1",
                table: "AssignmentVariant");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentVariant_AssignmentId1",
                table: "AssignmentVariant");

            migrationBuilder.DropColumn(
                name: "AssignmentId1",
                table: "AssignmentVariant");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignmentId",
                table: "AssignmentVariant",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentVariant_AssignmentId",
                table: "AssignmentVariant",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentVariant_Assignments_AssignmentId",
                table: "AssignmentVariant",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentVariant_Assignments_AssignmentId",
                table: "AssignmentVariant");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentVariant_AssignmentId",
                table: "AssignmentVariant");

            migrationBuilder.AlterColumn<string>(
                name: "AssignmentId",
                table: "AssignmentVariant",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "AssignmentId1",
                table: "AssignmentVariant",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentVariant_AssignmentId1",
                table: "AssignmentVariant",
                column: "AssignmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentVariant_Assignments_AssignmentId1",
                table: "AssignmentVariant",
                column: "AssignmentId1",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
