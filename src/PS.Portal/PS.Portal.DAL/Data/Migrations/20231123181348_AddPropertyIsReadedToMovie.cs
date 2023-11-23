using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Portal.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyIsReadedToMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReaded",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReaded",
                table: "Movies");
        }
    }
}
