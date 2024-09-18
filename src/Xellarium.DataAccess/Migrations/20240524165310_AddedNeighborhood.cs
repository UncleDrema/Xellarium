using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Xellarium.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedNeighborhood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GenericRule_NeighborhoodType",
                table: "Rules",
                newName: "NeighborhoodId");

            migrationBuilder.CreateTable(
                name: "Neighborhood",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Offsets = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Neighborhood", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_NeighborhoodId",
                table: "Rules",
                column: "NeighborhoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Neighborhood_NeighborhoodId",
                table: "Rules",
                column: "NeighborhoodId",
                principalTable: "Neighborhood",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_Neighborhood_NeighborhoodId",
                table: "Rules");

            migrationBuilder.DropTable(
                name: "Neighborhood");

            migrationBuilder.DropIndex(
                name: "IX_Rules_NeighborhoodId",
                table: "Rules");

            migrationBuilder.RenameColumn(
                name: "NeighborhoodId",
                table: "Rules",
                newName: "GenericRule_NeighborhoodType");
        }
    }
}
