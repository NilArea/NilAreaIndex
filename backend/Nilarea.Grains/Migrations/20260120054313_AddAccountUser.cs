using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NilArea.Grains.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "AccountUser",
                schema: "NilArea",
                newName: "AccountUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "NilArea");

            migrationBuilder.RenameTable(
                name: "AccountUser",
                newName: "AccountUser",
                newSchema: "NilArea");
        }
    }
}
