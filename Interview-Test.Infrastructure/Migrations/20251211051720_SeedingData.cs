using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Interview_Test.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RoleTb",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "pick operation" },
                    { 2, "pack operation" },
                    { 3, "document operation" }
                });

            migrationBuilder.InsertData(
                table: "PermissionTb",
                columns: new[] { "PermissionId", "Permission", "RoleId" },
                values: new object[,]
                {
                    { 1L, "1-01-picking-info", 1 },
                    { 2L, "1-02-picking-start", 1 },
                    { 3L, "1-03-picking-confirm", 1 },
                    { 4L, "1-04-picking-report", 1 },
                    { 5L, "2-01-packing-info", 2 },
                    { 6L, "2-02-packing-start", 2 },
                    { 7L, "2-03-packing-confirm", 2 },
                    { 8L, "2-04-packing-report", 2 },
                    { 9L, "1-04-picking-report", 3 },
                    { 10L, "2-04-packing-report", 3 },
                    { 11L, "3-01-printing-label", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "PermissionTb",
                keyColumn: "PermissionId",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "RoleTb",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RoleTb",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RoleTb",
                keyColumn: "RoleId",
                keyValue: 3);
        }
    }
}
