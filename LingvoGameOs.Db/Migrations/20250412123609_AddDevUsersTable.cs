using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddDevUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DevUserId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DevUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Login = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevUsers", x => x.Id);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_DevUsers_DevUserId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "DevUsers");

            migrationBuilder.DropIndex(
                name: "IX_Games_DevUserId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DevUserId",
                table: "Games");
        }
    }
}
