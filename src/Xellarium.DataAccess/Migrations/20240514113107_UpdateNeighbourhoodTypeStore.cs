using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xellarium.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNeighbourhoodTypeStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NeighbourhoodType",
                table: "Rules",
                newName: "GenericRule_NeighborhoodType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GenericRule_NeighborhoodType",
                table: "Rules",
                newName: "NeighbourhoodType");
        }
    }
}
