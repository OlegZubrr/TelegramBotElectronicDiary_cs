using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotEFCore.Migrations
{
    /// <inheritdoc />
    public partial class addCurrentStudentIdtoteacherEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStudentId",
                table: "Teachers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStudentId",
                table: "Teachers");
        }
    }
}
