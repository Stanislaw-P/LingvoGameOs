using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorStringIdInGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorStringId",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AuthorStringId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "", new DateTime(2025, 5, 4, 19, 44, 40, 784, DateTimeKind.Utc).AddTicks(6613), new DateTime(2025, 5, 4, 19, 44, 40, 784, DateTimeKind.Utc).AddTicks(6609) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AuthorStringId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "", new DateTime(2025, 5, 4, 19, 44, 40, 784, DateTimeKind.Utc).AddTicks(6618), new DateTime(2025, 5, 4, 19, 44, 40, 784, DateTimeKind.Utc).AddTicks(6618) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AuthorStringId", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "", new DateTime(2025, 5, 4, 19, 44, 40, 784, DateTimeKind.Utc).AddTicks(6621), new DateTime(2025, 5, 4, 19, 44, 40, 784, DateTimeKind.Utc).AddTicks(6621) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorStringId",
                table: "Games");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 4, 19, 40, 32, 925, DateTimeKind.Utc).AddTicks(9428), new DateTime(2025, 5, 4, 19, 40, 32, 925, DateTimeKind.Utc).AddTicks(9425) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 4, 19, 40, 32, 925, DateTimeKind.Utc).AddTicks(9433), new DateTime(2025, 5, 4, 19, 40, 32, 925, DateTimeKind.Utc).AddTicks(9433) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 4, 19, 40, 32, 925, DateTimeKind.Utc).AddTicks(9437), new DateTime(2025, 5, 4, 19, 40, 32, 925, DateTimeKind.Utc).AddTicks(9436) });
        }
    }
}
