using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Sites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Sites_CompanyId",
                table: "Sites",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Companies_CompanyId",
                table: "Sites",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Companies_CompanyId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_CompanyId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Sites");
        }
    }
}
