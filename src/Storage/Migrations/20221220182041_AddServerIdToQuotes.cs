using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaKeeper.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddServerIdToQuotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreatedOn",
                table: "Quotes",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Quotes");
        }
    }
}
