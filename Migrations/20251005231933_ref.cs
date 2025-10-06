using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habit_Battles.Migrations
{
    /// <inheritdoc />
    public partial class @ref : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefId",
                table: "Battles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefId",
                table: "Battles");
        }
    }
}
