using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaintApp_Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed" },
                values: new object[] { new DateTime(2025, 12, 6, 22, 45, 3, 323, DateTimeKind.Local).AddTicks(1736), new DateTime(2025, 12, 6, 22, 45, 3, 323, DateTimeKind.Local).AddTicks(1761) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed" },
                values: new object[] { new DateTime(2025, 12, 6, 21, 16, 10, 345, DateTimeKind.Local).AddTicks(4410), new DateTime(2025, 12, 6, 21, 16, 10, 345, DateTimeKind.Local).AddTicks(4421) });
        }
    }
}
