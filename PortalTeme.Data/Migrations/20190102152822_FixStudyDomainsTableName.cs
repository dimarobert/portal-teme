using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class FixStudyDomainsTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_StudyDomain_DomainId",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyDomain",
                table: "StudyDomain");

            migrationBuilder.RenameTable(
                name: "StudyDomain",
                newName: "StudyDomains");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyDomains",
                table: "StudyDomains",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_StudyDomains_DomainId",
                table: "Groups",
                column: "DomainId",
                principalTable: "StudyDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_StudyDomains_DomainId",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyDomains",
                table: "StudyDomains");

            migrationBuilder.RenameTable(
                name: "StudyDomains",
                newName: "StudyDomain");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyDomain",
                table: "StudyDomain",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_StudyDomain_DomainId",
                table: "Groups",
                column: "DomainId",
                principalTable: "StudyDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
