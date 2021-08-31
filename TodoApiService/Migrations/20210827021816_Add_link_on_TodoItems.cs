using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoApiService.Migrations
{
    public partial class Add_link_on_TodoItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItem_Accounts_AccountId",
                table: "TodoItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoItem",
                table: "TodoItem");

            migrationBuilder.RenameTable(
                name: "TodoItem",
                newName: "TodoItems");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItem_AccountId",
                table: "TodoItems",
                newName: "IX_TodoItems_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItems",
                table: "TodoItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Accounts_AccountId",
                table: "TodoItems",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Accounts_AccountId",
                table: "TodoItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoItems",
                table: "TodoItems");

            migrationBuilder.RenameTable(
                name: "TodoItems",
                newName: "TodoItem");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItems_AccountId",
                table: "TodoItem",
                newName: "IX_TodoItem_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItem",
                table: "TodoItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItem_Accounts_AccountId",
                table: "TodoItem",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
