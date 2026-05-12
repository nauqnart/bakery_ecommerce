namespace StitchArtisan.Backend.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string BaseDescription { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public int ProductId { get; set; }
    }
}
