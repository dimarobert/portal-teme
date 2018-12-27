using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class SeedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "36f16a57-5b33-4bc3-99e8-51d61595ec2f", "cf3a5c72-0813-415b-879b-685eb7942e64", "Admin", "ADMIN" },
                    { "ea74f168-a8e8-4267-a078-a1c2d6ef2251", "7706b80b-e1fc-4b06-870c-2a02a72b5684", "Professor", "PROFESSOR" },
                    { "bd9d8efc-b46d-40e4-b0e2-5ce581d2bd0b", "1969f591-fd47-409f-867e-0d0fdec0c584", "Assistant", "ASSISTANT" },
                    { "a42fcfeb-29d5-4f8e-9c31-a174b4388e02", "39b8570f-c1ba-4e7b-9c64-49c0d15bf96f", "Student", "STUDENT" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36f16a57-5b33-4bc3-99e8-51d61595ec2f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a42fcfeb-29d5-4f8e-9c31-a174b4388e02");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd9d8efc-b46d-40e4-b0e2-5ce581d2bd0b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ea74f168-a8e8-4267-a078-a1c2d6ef2251");
        }
    }
}
