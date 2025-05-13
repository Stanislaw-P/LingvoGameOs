using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAuthorFromGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b45ef5bd-da4b-41f8-8c78-38e60e4c5291");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Description", "Email", "EmailConfirmed", "ImageURL", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Surname", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a7038988-99ad-48b5-8370-e2214e2d6055", 0, "eddb262c-1bf9-4f47-a905-6c655195101a", null, "MaratTest@mail.ru", false, null, false, null, "Марат", null, null, null, null, false, "11238855-df1c-4742-9bf9-6ef5184e539c", "Какой-то", false, "MaratTest@mail.ru" });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AuthorId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "a7038988-99ad-48b5-8370-e2214e2d6055", new DateTime(2025, 5, 13, 17, 12, 22, 195, DateTimeKind.Utc).AddTicks(2807), new DateTime(2025, 5, 13, 17, 12, 22, 195, DateTimeKind.Utc).AddTicks(2803) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AuthorId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "a7038988-99ad-48b5-8370-e2214e2d6055", new DateTime(2025, 5, 13, 17, 12, 22, 195, DateTimeKind.Utc).AddTicks(2812), new DateTime(2025, 5, 13, 17, 12, 22, 195, DateTimeKind.Utc).AddTicks(2812) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AuthorId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "a7038988-99ad-48b5-8370-e2214e2d6055", new DateTime(2025, 5, 13, 17, 12, 22, 195, DateTimeKind.Utc).AddTicks(2816), new DateTime(2025, 5, 13, 17, 12, 22, 195, DateTimeKind.Utc).AddTicks(2815) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a7038988-99ad-48b5-8370-e2214e2d6055");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Description", "Email", "EmailConfirmed", "ImageURL", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Surname", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b45ef5bd-da4b-41f8-8c78-38e60e4c5291", 0, "fe4bd986-5da8-4d47-a219-071e610d50c6", null, "MaratTest@mail.ru", false, null, false, null, "Марат", null, null, null, null, false, "7bb38f54-ef2a-4653-8943-df34479d4303", "Какой-то", false, "MaratTest@mail.ru" });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AuthorId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "b45ef5bd-da4b-41f8-8c78-38e60e4c5291", new DateTime(2025, 5, 13, 16, 41, 52, 82, DateTimeKind.Utc).AddTicks(4475), new DateTime(2025, 5, 13, 16, 41, 52, 82, DateTimeKind.Utc).AddTicks(4470) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AuthorId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "b45ef5bd-da4b-41f8-8c78-38e60e4c5291", new DateTime(2025, 5, 13, 16, 41, 52, 82, DateTimeKind.Utc).AddTicks(4480), new DateTime(2025, 5, 13, 16, 41, 52, 82, DateTimeKind.Utc).AddTicks(4479) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AuthorId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "b45ef5bd-da4b-41f8-8c78-38e60e4c5291", new DateTime(2025, 5, 13, 16, 41, 52, 82, DateTimeKind.Utc).AddTicks(4483), new DateTime(2025, 5, 13, 16, 41, 52, 82, DateTimeKind.Utc).AddTicks(4483) });
        }
    }
}
