using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotEFCore.Migrations
{
    /// <inheritdoc />
    public partial class updategroupentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupEntityTeacherEntity");

            migrationBuilder.DropColumn(
                name: "TeachersIds",
                table: "Groups");

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherEntityId",
                table: "Groups",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TeacherEntityId",
                table: "Groups",
                column: "TeacherEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Teachers_TeacherEntityId",
                table: "Groups",
                column: "TeacherEntityId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Teachers_TeacherEntityId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TeacherEntityId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TeacherEntityId",
                table: "Groups");

            migrationBuilder.AddColumn<string>(
                name: "TeachersIds",
                table: "Groups",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

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
    }
}
