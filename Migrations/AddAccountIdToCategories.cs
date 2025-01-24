using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace coa_wallet.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AccountId",
                table: "Categories",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Accounts_AccountId",
                table: "Categories",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Accounts_AccountId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_AccountId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Categories");
        }
    }
}