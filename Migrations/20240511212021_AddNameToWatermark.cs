using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pictoflow_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToWatermark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Watermarks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Galleries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Galleries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Watermarks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Galleries");
        }
    }
}
