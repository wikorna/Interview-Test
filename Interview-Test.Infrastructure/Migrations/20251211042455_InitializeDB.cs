using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview_Test.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitializeDB : Migration
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
                    RoleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
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
                    UserId = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
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
                    Permission = table.Column<string>(type: "text", maxLength: 200, nullable: false),
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
                    Age = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileTb", x => x.ProfileId);
                    table.ForeignKey(
                        name: "FK_UserProfileTb_UserTb_Id",
                        column: x => x.Id,
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

            migrationBuilder.CreateIndex(
                name: "IX_PermissionTb_RoleId",
                table: "PermissionTb",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileTb_Id",
                table: "UserProfileTb",
                column: "Id",
                unique: true);

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
