using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropToGameSkillLearning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSkillLearning_SkillsLearning_GameTypeId",
                table: "GameSkillLearning");

            migrationBuilder.RenameColumn(
                name: "GameTypeId",
                table: "GameSkillLearning",
                newName: "SkillLearningId");

            migrationBuilder.RenameIndex(
                name: "IX_GameSkillLearning_GameTypeId",
                table: "GameSkillLearning",
                newName: "IX_GameSkillLearning_SkillLearningId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSkillLearning_SkillsLearning_SkillLearningId",
                table: "GameSkillLearning",
                column: "SkillLearningId",
                principalTable: "SkillsLearning",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSkillLearning_SkillsLearning_SkillLearningId",
                table: "GameSkillLearning");

            migrationBuilder.RenameColumn(
                name: "SkillLearningId",
                table: "GameSkillLearning",
                newName: "GameTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_GameSkillLearning_SkillLearningId",
                table: "GameSkillLearning",
                newName: "IX_GameSkillLearning_GameTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSkillLearning_SkillsLearning_GameTypeId",
                table: "GameSkillLearning",
                column: "GameTypeId",
                principalTable: "SkillsLearning",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
