using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VCCommandAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HowTo = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Line = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Platform = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    //CSG ADDED CUSTOM REFERENCED VIA [DatabaseGenerated(DatabaseGeneratedOption.Computed)] IN Command.cs
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()") 
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commands");
        }
    }
}
