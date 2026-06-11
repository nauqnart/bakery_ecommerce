using StitchArtisan.Backend.DTOs;

namespace StitchArtisan.Backend.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<object> GetRevenueChartAsync();
        Task<object> GetTopProductsChartAsync();
        Task<object> GetBottomProductsChartAsync();
        Task<object> GetChartDataAsync();
    }
}
