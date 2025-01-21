using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotEFCore.Migrations
{
    /// <inheritdoc />
    public partial class updateTeacherEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Groups_GroupId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_GroupId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Teachers");

            migrationBuilder.CreateTable(
                name: "GroupEntityTeacherEntity",
                columns: table => new
                {
                    GroupsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TeachersId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupEntityTeacherEntity", x => new { x.GroupsId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_GroupEntityTeacherEntity_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupEntityTeacherEntity_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GroupEntityTeacherEntity_TeachersId",
                table: "GroupEntityTeacherEntity",
                column: "TeachersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupEntityTeacherEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Teachers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_GroupId",
                table: "Teachers",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Groups_GroupId",
                table: "Teachers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
