namespace StitchArtisan.Backend.DTOs
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
    }
}
