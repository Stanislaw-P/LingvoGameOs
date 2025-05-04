using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7754), new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7753) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7757), new DateTime(2025, 5, 1, 6, 39, 46, 28, DateTimeKind.Utc).AddTicks(7756) });
        }
    }
}
