using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pictoflow_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGalleryAndPhotoModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Photos",
                newName: "WatermarkedImagePath");

            migrationBuilder.AddColumn<string>(
                name: "HighResImagePath",
                table: "Photos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "WatermarkStyle",
                table: "Galleries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighResImagePath",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "WatermarkStyle",
                table: "Galleries");

            migrationBuilder.RenameColumn(
                name: "WatermarkedImagePath",
                table: "Photos",
                newName: "ImagePath");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Photos",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
