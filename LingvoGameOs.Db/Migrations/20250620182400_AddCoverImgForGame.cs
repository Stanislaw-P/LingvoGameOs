using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverImgForGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagesURLs",
                table: "Games",
                newName: "ImagesPaths");

            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "ImagesPaths",
                table: "Games",
                newName: "ImagesURLs");
        }
    }
}
