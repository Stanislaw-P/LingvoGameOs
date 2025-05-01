using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGameData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7745), new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7742) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate", "Raiting" },
                values: new object[] { new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7754), new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7753), 4.4000000000000004 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CoverImageURL", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "/img/games/gameplay-animal.png", new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7757), new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7756) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3993), new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3991) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate", "Raiting" },
                values: new object[] { new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3997), new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3997), 4.2000000000000002 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CoverImageURL", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "/img/games/gameplay-test.png", new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(4000), new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(4000) });
        }
    }
}
