using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixNameRable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameGameType_GameTypes_GameTypeId",
                table: "GameGameType");

            migrationBuilder.DropForeignKey(
                name: "FK_GameGameType_Games_GameId",
                table: "GameGameType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameGameType",
                table: "GameGameType");

            migrationBuilder.RenameTable(
                name: "GameGameType",
                newName: "GameSkillLearning");

            migrationBuilder.RenameIndex(
                name: "IX_GameGameType_GameTypeId",
                table: "GameSkillLearning",
                newName: "IX_GameSkillLearning_GameTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameSkillLearning",
                table: "GameSkillLearning",
                columns: new[] { "GameId", "GameTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameSkillLearning_GameTypes_GameTypeId",
                table: "GameSkillLearning",
                column: "GameTypeId",
                principalTable: "GameTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSkillLearning_Games_GameId",
                table: "GameSkillLearning",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSkillLearning_GameTypes_GameTypeId",
                table: "GameSkillLearning");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSkillLearning_Games_GameId",
                table: "GameSkillLearning");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameSkillLearning",
                table: "GameSkillLearning");

            migrationBuilder.RenameTable(
                name: "GameSkillLearning",
                newName: "GameGameType");

            migrationBuilder.RenameIndex(
                name: "IX_GameSkillLearning_GameTypeId",
                table: "GameGameType",
                newName: "IX_GameGameType_GameTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameGameType",
                table: "GameGameType",
                columns: new[] { "GameId", "GameTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameGameType_GameTypes_GameTypeId",
                table: "GameGameType",
                column: "GameTypeId",
                principalTable: "GameTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameGameType_Games_GameId",
                table: "GameGameType",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
