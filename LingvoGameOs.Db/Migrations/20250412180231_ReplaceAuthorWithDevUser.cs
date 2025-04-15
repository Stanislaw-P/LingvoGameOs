using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceAuthorWithDevUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_DevUsers_DevUserId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_User_AuthorId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Games_DevUserId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DevUserId",
                table: "Games");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_DevUsers_AuthorId",
                table: "Games",
                column: "AuthorId",
                principalTable: "DevUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_DevUsers_AuthorId",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "DevUserId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_DevUserId",
                table: "Games",
                column: "DevUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_DevUsers_DevUserId",
                table: "Games",
                column: "DevUserId",
                principalTable: "DevUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_User_AuthorId",
                table: "Games",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
