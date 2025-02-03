using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotEFCore.Migrations
{
    /// <inheritdoc />
    public partial class addmessageIdstouserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MesssageIds",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MesssageIds",
                table: "Users");
        }
    }
}
