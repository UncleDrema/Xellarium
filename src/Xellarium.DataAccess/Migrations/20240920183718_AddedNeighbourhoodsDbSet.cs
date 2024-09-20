using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xellarium.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedNeighbourhoodsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_Neighborhood_NeighborhoodId",
                table: "Rules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Neighborhood",
                table: "Neighborhood");

            migrationBuilder.RenameTable(
                name: "Neighborhood",
                newName: "Neighborhoods");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Neighborhoods",
                table: "Neighborhoods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Neighborhoods_NeighborhoodId",
                table: "Rules",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_Neighborhoods_NeighborhoodId",
                table: "Rules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Neighborhoods",
                table: "Neighborhoods");

            migrationBuilder.RenameTable(
                name: "Neighborhoods",
                newName: "Neighborhood");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Neighborhood",
                table: "Neighborhood",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Neighborhood_NeighborhoodId",
                table: "Rules",
                column: "NeighborhoodId",
                principalTable: "Neighborhood",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
