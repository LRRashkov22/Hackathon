using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passalong.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDesiredSwap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DesiredSwap",
                table: "Listings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredSwap",
                table: "Listings");
        }
    }
}
