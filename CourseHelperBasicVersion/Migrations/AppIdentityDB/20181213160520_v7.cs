using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CourseHelperBasicVersion.Migrations.AppIdentityDB
{
    public partial class v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }
    }
}
