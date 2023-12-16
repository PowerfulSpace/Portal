using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Portal.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditNamePropertyReleaseYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YearShown",
                table: "Movies",
                newName: "ReleaseYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReleaseYear",
                table: "Movies",
                newName: "YearShown");
        }
    }
}
