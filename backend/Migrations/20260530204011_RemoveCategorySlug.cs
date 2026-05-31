using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StitchArtisan.Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategorySlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_categories_slug",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "categories",
                type: "nvarchar(191)",
                maxLength: 191,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_categories_slug",
                table: "categories",
                column: "slug",
                unique: true);
        }
    }
}
