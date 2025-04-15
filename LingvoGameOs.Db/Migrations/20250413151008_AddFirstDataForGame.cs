using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstDataForGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameTypes_Games_GameId",
                table: "GameTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Technologys_Games_GameId",
                table: "Technologys");

            migrationBuilder.DropIndex(
                name: "IX_Technologys_GameId",
                table: "Technologys");

            migrationBuilder.DropIndex(
                name: "IX_GameTypes_GameId",
                table: "GameTypes");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Technologys");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GameTypes");

            migrationBuilder.CreateTable(
                name: "GameGameType",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGameType", x => new { x.GameId, x.GameTypeId });
                    table.ForeignKey(
                        name: "FK_GameGameType_GameTypes_GameTypeId",
                        column: x => x.GameTypeId,
                        principalTable: "GameTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGameType_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DevUsers",
                columns: new[] { "Id", "Description", "Email", "Login", "Name", "Password" },
                values: new object[] { 1, "Студент яндекс лицея. Лучший разработчик по версии журнала Babushka", "AlanTest@mail.ru", "AlanTest", "Алан", "_Aa123456" });

            migrationBuilder.InsertData(
                table: "GameTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Словарный запас" },
                    { 2, "Грамматика" },
                    { 3, "Аудирование" },
                    { 4, "Чтение" },
                    { 5, "Говорение" },
                    { 6, "Головоломка" }
                });

            migrationBuilder.InsertData(
                table: "LanguageLevels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Начинающий" },
                    { 2, "Средний" },
                    { 3, "Продвинутый" }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Web" },
                    { 2, "Desktop" }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AuthorId", "CoverImageURL", "Description", "GamePlatformId", "GameURL", "LanguageLevelId", "LastUpdateDate", "NumberDownloads", "PublicationDate", "Raiting", "Title" },
                values: new object[,]
                {
                    { 1, 1, "/img/games/mountain labyrinth-banner.png", "Отправляйтесь в увлекательное путешествие, проходите сказочные лабиринты и создавайте собственные в удобном редакторе.", 2, "/home/index", 1, new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(742), 1000, new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(738), 4.5999999999999996, "Горный лабиринт" },
                    { 2, 1, "/img/games/art-object-banner.png", "Супер интересная викторина для компании. Поможет найти арт пространства и расскажет о них много интересного.", 1, "/home/index", 2, new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(753), 2241, new DateTime(2025, 4, 13, 15, 10, 5, 219, DateTimeKind.Utc).AddTicks(752), 4.2000000000000002, "Тур-викторина 'Арт объекты Осетии'" }
                });

            migrationBuilder.InsertData(
                table: "GameGameType",
                columns: new[] { "GameId", "GameTypeId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 2, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGameType_GameTypeId",
                table: "GameGameType",
                column: "GameTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameGameType");

            migrationBuilder.DeleteData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LanguageLevels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DevUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LanguageLevels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LanguageLevels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Platforms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Platforms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Technologys",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "GameTypes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technologys_GameId",
                table: "Technologys",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTypes_GameId",
                table: "GameTypes",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameTypes_Games_GameId",
                table: "GameTypes",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Technologys_Games_GameId",
                table: "Technologys",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
