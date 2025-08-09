using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyPedingGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillsLearning_PendingGames_PendingGameId",
                table: "SkillsLearning");

            migrationBuilder.DropIndex(
                name: "IX_SkillsLearning_PendingGameId",
                table: "SkillsLearning");

            migrationBuilder.DropColumn(
                name: "PendingGameId",
                table: "SkillsLearning");

            migrationBuilder.CreateTable(
                name: "PendingGameSkillLearning",
                columns: table => new
                {
                    PendingGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillLearningId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingGameSkillLearning", x => new { x.PendingGameId, x.SkillLearningId });
                    table.ForeignKey(
                        name: "FK_PendingGameSkillLearning_PendingGames_PendingGameId",
                        column: x => x.PendingGameId,
                        principalTable: "PendingGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingGameSkillLearning_SkillsLearning_SkillLearningId",
                        column: x => x.SkillLearningId,
                        principalTable: "SkillsLearning",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingGameSkillLearning_SkillLearningId",
                table: "PendingGameSkillLearning",
                column: "SkillLearningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingGameSkillLearning");

            migrationBuilder.AddColumn<int>(
                name: "PendingGameId",
                table: "SkillsLearning",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillsLearning_PendingGameId",
                table: "SkillsLearning",
                column: "PendingGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillsLearning_PendingGames_PendingGameId",
                table: "SkillsLearning",
                column: "PendingGameId",
                principalTable: "PendingGames",
                principalColumn: "Id");
        }
    }
}
