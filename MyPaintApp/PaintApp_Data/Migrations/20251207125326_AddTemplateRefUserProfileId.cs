using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaintApp_Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateRefUserProfileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "ShapeTemplates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed" },
                values: new object[] { new DateTime(2025, 12, 7, 19, 53, 22, 74, DateTimeKind.Local).AddTicks(1914), new DateTime(2025, 12, 7, 19, 53, 22, 74, DateTimeKind.Local).AddTicks(1936) });

            migrationBuilder.CreateIndex(
                name: "IX_ShapeTemplates_UserProfileId",
                table: "ShapeTemplates",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShapeTemplates_UserProfiles_UserProfileId",
                table: "ShapeTemplates",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShapeTemplates_UserProfiles_UserProfileId",
                table: "ShapeTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ShapeTemplates_UserProfileId",
                table: "ShapeTemplates");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "ShapeTemplates");

            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed" },
                values: new object[] { new DateTime(2025, 12, 7, 5, 30, 38, 355, DateTimeKind.Local).AddTicks(8580), new DateTime(2025, 12, 7, 5, 30, 38, 355, DateTimeKind.Local).AddTicks(8591) });
        }
    }
}
