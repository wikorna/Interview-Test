using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Interview_Test.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitializeDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleTb",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTb", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "UserTb",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "varchar(20)", nullable: false),
                    Username = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionTb",
                columns: table => new
                {
                    PermissionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Permission = table.Column<string>(type: "varchar(100)", maxLength: 200, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionTb", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_PermissionTb_RoleTb_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleTb",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfileTb",
                columns: table => new
                {
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(100)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(100)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileTb", x => x.ProfileId);
                    table.ForeignKey(
                        name: "FK_UserProfileTb_UserTb_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "UserTb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleMappingTb",
                columns: table => new
                {
                    UserRoleMappingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleMappingTb", x => x.UserRoleMappingId);
                    table.ForeignKey(
                        name: "FK_UserRoleMappingTb_RoleTb_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleTb",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleMappingTb_UserTb_UserId",
                        column: x => x.UserId,
                        principalTable: "UserTb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    { 9L, "3-01-printing-label", 3 },
                    { 10L, "1-04-picking-report", 3 },
                    { 11L, "2-04-packing-report", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionTb_RoleId",
                table: "PermissionTb",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappingTb_RoleId",
                table: "UserRoleMappingTb",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappingTb_UserId_RoleId",
                table: "UserRoleMappingTb",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTb_UserId",
                table: "UserTb",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionTb");

            migrationBuilder.DropTable(
                name: "UserProfileTb");

            migrationBuilder.DropTable(
                name: "UserRoleMappingTb");

            migrationBuilder.DropTable(
                name: "RoleTb");

            migrationBuilder.DropTable(
                name: "UserTb");
        }
    }
}
