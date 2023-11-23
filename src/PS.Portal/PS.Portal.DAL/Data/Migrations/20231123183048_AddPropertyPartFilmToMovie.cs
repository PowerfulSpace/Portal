using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Portal.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyPartFilmToMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartFilm",
                table: "Movies",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartFilm",
                table: "Movies");
        }
    }
}
