using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Login = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Login = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surname = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technologys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LanguageLevelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Raiting = table.Column<double>(type: "REAL", nullable: false),
                    CoverImageURL = table.Column<string>(type: "TEXT", nullable: false),
                    GameURL = table.Column<string>(type: "TEXT", nullable: false),
                    GamePlatformId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberDownloads = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_DevUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "DevUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_LanguageLevels_LanguageLevelId",
                        column: x => x.LanguageLevelId,
                        principalTable: "LanguageLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Platforms_GamePlatformId",
                        column: x => x.GamePlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                values: new object[] { 1, "Студент яндекс лицея. Лучший разработчик по версии журнала Babushka", "MaratTest@mail.ru", "MaratTest", "Марат", "_Aa123456" });

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
                    { 1, "Web-Desktop" },
                    { 2, "Desktop" },
                    { 3, "Web-Mobile" }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AuthorId", "CoverImageURL", "Description", "GamePlatformId", "GameURL", "LanguageLevelId", "LastUpdateDate", "NumberDownloads", "PublicationDate", "Raiting", "Title" },
                values: new object[,]
                {
                    { 1, 1, "/img/games/mountain labyrinth-banner.png", "Отправляйтесь в увлекательное путешествие, проходите сказочные лабиринты и создавайте собственные в удобном редакторе.", 2, "/home/index", 1, new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3993), 1000, new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3991), 4.5999999999999996, "Горный лабиринт" },
                    { 2, 1, "/img/games/art-object-banner.png", "Супер интересная викторина для компании. Поможет найти арт пространства и расскажет о них много интересного.", 1, "/home/index", 1, new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3997), 2241, new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(3997), 4.2000000000000002, "Тур-викторина 'Арт объекты Осетии'" },
                    { 3, 1, "/img/games/gameplay-test.png", "Игра состоит из двух уровней никак не связанных друг с другом. После открытия сайта пользователь попадает на главное окно. Там он может ознакомится с правилами игры, а также просмотреть список лидеров и увидеть свой уровень достижений И зарегистрироваться/войти в аккаунт.", 3, "http://84.201.144.125:5001", 2, new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(4000), 5, new DateTime(2025, 4, 30, 21, 21, 35, 535, DateTimeKind.Utc).AddTicks(4000), 4.2000000000000002, "Собери животное" }
                });

            migrationBuilder.InsertData(
                table: "GameGameType",
                columns: new[] { "GameId", "GameTypeId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 2, 4 },
                    { 3, 1 },
                    { 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGameType_GameTypeId",
                table: "GameGameType",
                column: "GameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AuthorId",
                table: "Games",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GamePlatformId",
                table: "Games",
                column: "GamePlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_LanguageLevelId",
                table: "Games",
                column: "LanguageLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameGameType");

            migrationBuilder.DropTable(
                name: "PlayerUsers");

            migrationBuilder.DropTable(
                name: "Technologys");

            migrationBuilder.DropTable(
                name: "GameTypes");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "DevUsers");

            migrationBuilder.DropTable(
                name: "LanguageLevels");

            migrationBuilder.DropTable(
                name: "Platforms");
        }
    }
}
