using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xellarium.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransitionStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenericRule_StateTransitionsJson",
                table: "Rules");

            migrationBuilder.AddColumn<string>(
                name: "GenericRule_StateTransitions",
                table: "Rules",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenericRule_StateTransitions",
                table: "Rules");

            migrationBuilder.AddColumn<string>(
                name: "GenericRule_StateTransitionsJson",
                table: "Rules",
                type: "text",
                nullable: true);
        }
    }
}
