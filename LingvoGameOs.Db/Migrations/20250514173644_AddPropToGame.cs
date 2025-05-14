using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPropToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Raiting",
                table: "Games",
                newName: "RaitingTeachers");

            migrationBuilder.AddColumn<double>(
                name: "RaitingPlayers",
                table: "Games",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate", "RaitingPlayers", "RaitingTeachers" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3585), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3582), 4.5999999999999996, 4.7999999999999998 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate", "RaitingPlayers", "RaitingTeachers" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3589), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3589), 4.4000000000000004, 4.0 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastUpdateDate", "PublicationDate", "RaitingPlayers", "RaitingTeachers" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3592), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3591), 4.2000000000000002, 4.2999999999999998 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "LastUpdateDate", "PublicationDate", "RaitingPlayers", "RaitingTeachers" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3594), new DateTime(2025, 5, 14, 17, 36, 44, 82, DateTimeKind.Utc).AddTicks(3594), 5.0, 4.9000000000000004 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RaitingPlayers",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "RaitingTeachers",
                table: "Games",
                newName: "Raiting");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate", "Raiting" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2528), new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2525), 4.5999999999999996 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate", "Raiting" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2532), new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2531), 4.4000000000000004 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastUpdateDate", "PublicationDate", "Raiting" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2534), new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2534), 4.2000000000000002 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "LastUpdateDate", "PublicationDate", "Raiting" },
                values: new object[] { new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2536), new DateTime(2025, 5, 14, 17, 28, 58, 749, DateTimeKind.Utc).AddTicks(2536), 5.0 });
        }
    }
}
