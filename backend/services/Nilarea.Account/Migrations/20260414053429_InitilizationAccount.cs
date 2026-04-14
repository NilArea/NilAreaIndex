using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NilArea.Account.Migrations
{
    /// <inheritdoc />
    public partial class InitilizationAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountUser",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    PasswordSaltHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    UserName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    DeleteAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    UpdateAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountUser_UserId", x => x.UserId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AccountUser_CreatedAt",
                table: "AccountUser",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountUser_Email",
                table: "AccountUser",
                columns: new[] { "Email", "DeleteAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountUser_UpdateAt",
                table: "AccountUser",
                column: "UpdateAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountUser");
        }
    }
}
