using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StitchArtisan.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "variant_name",
                table: "product_variants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "variant_name",
                table: "product_variants");
        }
    }
}
