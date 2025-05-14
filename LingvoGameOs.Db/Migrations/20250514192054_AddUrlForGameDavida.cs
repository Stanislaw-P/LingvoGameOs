using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlForGameDavida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9170), new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9168) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9173), new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9173) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9176), new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9175) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GameURL", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "https://ossetian-crosswords.glitch.me/", new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9178), new DateTime(2025, 5, 14, 19, 20, 53, 984, DateTimeKind.Utc).AddTicks(9178) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "GameURL", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "скоро будет", new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3653), new DateTime(2025, 5, 14, 18, 12, 11, 856, DateTimeKind.Utc).AddTicks(3653) });
        }
    }
}
