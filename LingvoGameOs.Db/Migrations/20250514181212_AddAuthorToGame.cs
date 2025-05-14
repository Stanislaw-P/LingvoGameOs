using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_AspNetUsers_UserId1",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_UserId1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Games");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3644), new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3642) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3648), new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3648) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3650), new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3650) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3653), new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3653) });

            migrationBuilder.CreateIndex(
                name: "IX_Games_AuthorId",
                table: "Games",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_AspNetUsers_AuthorId",
                table: "Games",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_AspNetUsers_AuthorId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_AuthorId",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate", "UserId1" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3585), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3582), null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate", "UserId1" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3589), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3589), null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastUpdateDate", "PublicationDate", "UserId1" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3592), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3591), null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "LastUpdateDate", "PublicationDate", "UserId1" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3594), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3594), null });

            migrationBuilder.CreateIndex(
                name: "IX_Games_UserId1",
                table: "Games",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_AspNetUsers_UserId1",
                table: "Games",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
