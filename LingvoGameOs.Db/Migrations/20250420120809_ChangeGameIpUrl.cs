using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGameIpUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 4, 20, 12, 8, 7, 710, DateTimeKind.Utc).AddTicks(651), new DateTime(2025, 4, 20, 12, 8, 7, 710, DateTimeKind.Utc).AddTicks(644) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 4, 20, 12, 8, 7, 710, DateTimeKind.Utc).AddTicks(665), new DateTime(2025, 4, 20, 12, 8, 7, 710, DateTimeKind.Utc).AddTicks(665) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GameURL", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "http://84.201.144.125:5001", new DateTime(2025, 4, 20, 12, 8, 7, 710, DateTimeKind.Utc).AddTicks(668), new DateTime(2025, 4, 20, 12, 8, 7, 710, DateTimeKind.Utc).AddTicks(668) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9349), new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9347) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9353), new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9353) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GameURL", "LastUpdateDate", "PublicationDate" },
                values: new object[] { "http://158.160.142.70:5001", new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9356), new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9355) });
        }
    }
}
