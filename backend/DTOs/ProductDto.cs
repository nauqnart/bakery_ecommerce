namespace StitchArtisan.Backend.DTOs
{
    public class ProductVariantDto
    {
        public int? VariantId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> TierValues { get; set; } = new List<string>();
    }

    public class TierVariationDto
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new List<string>();
    }

    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? BaseDescription { get; set; }
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public List<TierVariationDto> TierVariations { get; set; } = new List<TierVariationDto>();
        public List<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public int ProductId { get; set; }
    }
}
