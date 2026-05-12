namespace StitchArtisan.Backend.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue     { get; set; }
        public int     TotalOrders      { get; set; }
        public int     PendingOrders    { get; set; }
        public int     TotalProducts    { get; set; }
        public int     LowStockCount    { get; set; }
        public int     ActiveCustomers  { get; set; }
        public decimal AvgOrderValue    { get; set; }
        public List<RecentOrderDto>     RecentOrders     { get; set; } = new();
        public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    }

    public class RecentOrderDto
    {
        public int      OrderId       { get; set; }
        public string   CustomerName  { get; set; } = string.Empty;
        public decimal  TotalPrice    { get; set; }
        public string   OrderStatus   { get; set; } = string.Empty;
        public string   PaymentStatus { get; set; } = string.Empty;
        public DateTime CreatedAt     { get; set; }
    }

    public class LowStockProductDto
    {
        public int     ProductId { get; set; }
        public string  Name      { get; set; } = string.Empty;
        public int     StockQty  { get; set; }
        public string? ImageUrl  { get; set; }
    }
}
