using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VCWebPortal.API.Migrations
{
    public partial class Create2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardholderName",
                table: "VCWebPortalTests",
                newName: "CardholderNameCSG");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardholderNameCSG",
                table: "VCWebPortalTests",
                newName: "CardholderName");
        }
    }
}
