using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class RenameNameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSkillLearning_GameTypes_GameTypeId",
                table: "GameSkillLearning");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameTypes",
                table: "GameTypes");

            migrationBuilder.RenameTable(
                name: "GameTypes",
                newName: "SkillsLearning");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillsLearning",
                table: "SkillsLearning",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSkillLearning_SkillsLearning_GameTypeId",
                table: "GameSkillLearning",
                column: "GameTypeId",
                principalTable: "SkillsLearning",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSkillLearning_SkillsLearning_GameTypeId",
                table: "GameSkillLearning");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillsLearning",
                table: "SkillsLearning");

            migrationBuilder.RenameTable(
                name: "SkillsLearning",
                newName: "GameTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameTypes",
                table: "GameTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSkillLearning_GameTypes_GameTypeId",
                table: "GameSkillLearning",
                column: "GameTypeId",
                principalTable: "GameTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
