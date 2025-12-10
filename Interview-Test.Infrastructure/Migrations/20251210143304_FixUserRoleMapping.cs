using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview_Test.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoleMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birthDate",
                table: "UserTb");

            migrationBuilder.DropColumn(
                name: "firstName",
                table: "UserTb");

            migrationBuilder.DropColumn(
                name: "lastName",
                table: "UserTb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "birthDate",
                table: "UserTb",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "firstName",
                table: "UserTb",
                type: "varchar(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "lastName",
                table: "UserTb",
                type: "varchar(40)",
                nullable: false,
                defaultValue: "");
        }
    }
}
