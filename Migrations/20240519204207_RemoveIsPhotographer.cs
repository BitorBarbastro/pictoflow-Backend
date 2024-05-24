using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pictoflow_Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsPhotographer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhotographer",
                table: "Users");

            migrationBuilder.AddColumn<decimal>(
                name: "IndividualPrice",
                table: "Galleries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Galleries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndividualPrice",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Galleries");

            migrationBuilder.AddColumn<bool>(
                name: "IsPhotographer",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
