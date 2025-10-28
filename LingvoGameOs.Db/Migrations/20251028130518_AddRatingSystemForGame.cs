using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoGameOs.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingSystemForGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RaitingPlayers",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "RaitingTeachers",
                table: "Games",
                newName: "AverageRaitingPlayers");

            migrationBuilder.AlterColumn<int>(
                name: "Port",
                table: "PendingGames",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "TotalRatingPoints",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalReviews",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalRatingPoints",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TotalReviews",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "AverageRaitingPlayers",
                table: "Games",
                newName: "RaitingTeachers");

            migrationBuilder.AlterColumn<int>(
                name: "Port",
                table: "PendingGames",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<double>(
                name: "RaitingPlayers",
                table: "Games",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
