using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoApiService.Migrations
{
    public partial class AddSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Accounts_AccountId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_AccountId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "RefreshTokens",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_SessionId",
                table: "RefreshTokens",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AccountId",
                table: "Sessions",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Sessions_SessionId",
                table: "RefreshTokens",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Sessions_SessionId",
                table: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_SessionId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AccountId",
                table: "RefreshTokens",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Accounts_AccountId",
                table: "RefreshTokens",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
