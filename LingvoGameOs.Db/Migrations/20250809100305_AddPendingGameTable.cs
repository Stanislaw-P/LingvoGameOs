using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingGameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PendingGameId",
                table: "SkillsLearning",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PendingGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 201, nullable: false),
                    Rules = table.Column<string>(type: "TEXT", nullable: false),
                    DispatchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<string>(type: "TEXT", nullable: false),
                    LanguageLevelId = table.Column<int>(type: "INTEGER", nullable: false),
                    CoverImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    ImagesPaths = table.Column<string>(type: "TEXT", nullable: false),
                    GameURL = table.Column<string>(type: "TEXT", nullable: true),
                    GamePlatformId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastMessage = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingGames_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingGames_LanguageLevels_LanguageLevelId",
                        column: x => x.LanguageLevelId,
                        principalTable: "LanguageLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingGames_Platforms_GamePlatformId",
                        column: x => x.GamePlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillsLearning_PendingGameId",
                table: "SkillsLearning",
                column: "PendingGameId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingGames_AuthorId",
                table: "PendingGames",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingGames_GamePlatformId",
                table: "PendingGames",
                column: "GamePlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingGames_LanguageLevelId",
                table: "PendingGames",
                column: "LanguageLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillsLearning_PendingGames_PendingGameId",
                table: "SkillsLearning",
                column: "PendingGameId",
                principalTable: "PendingGames",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillsLearning_PendingGames_PendingGameId",
                table: "SkillsLearning");

            migrationBuilder.DropTable(
                name: "PendingGames");

            migrationBuilder.DropIndex(
                name: "IX_SkillsLearning_PendingGameId",
                table: "SkillsLearning");

            migrationBuilder.DropColumn(
                name: "PendingGameId",
                table: "SkillsLearning");
        }
    }
}
