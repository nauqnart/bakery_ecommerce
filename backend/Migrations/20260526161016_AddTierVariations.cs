using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StitchArtisan.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTierVariations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tier_variations_json",
                table: "products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "variant_name",
                table: "product_variants",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "tier_values_json",
                table: "product_variants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tier_variations_json",
                table: "products");

            migrationBuilder.DropColumn(
                name: "tier_values_json",
                table: "product_variants");

            migrationBuilder.AlterColumn<string>(
                name: "variant_name",
                table: "product_variants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
