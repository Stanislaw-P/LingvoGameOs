using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddThirdGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AuthorId", "CoverImageURL", "Description", "GamePlatformId", "GameURL", "LanguageLevelId", "LastUpdateDate", "NumberDownloads", "PublicationDate", "Raiting", "Title" },
                values: new object[] { 3, 1, "/img/games/art-object-banner.png", "Игра состоит из двух уровней никак не связанных друг с другом. После открытия сайта пользователь попадает на главное окно. Там он может ознакомится с правилами игры, а также просмотреть список лидеров и увидеть свой уровень достижений И зарегистрироваться/войти в аккаунт.", 1, "http://158.160.142.70:5001", 2, new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9356), 5, new DateTime(2025, 4, 15, 11, 58, 57, 39, DateTimeKind.Utc).AddTicks(9355), 4.2000000000000002, "Собери животное" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(742), new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(738) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdateDate", "PublicationDate" },
                values: new object[] { new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(753), new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(752) });
        }
    }
}
