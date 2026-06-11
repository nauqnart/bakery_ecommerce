using Microsoft.AspNetCore.Mvc;
using StitchArtisan.Backend.Services;

namespace StitchArtisan.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync();
            return Ok(new { Success = true, Data = summary });
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueChart()
        {
            var data = await _dashboardService.GetRevenueChartAsync();
            return Ok(new { Success = true, Data = data });
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProductsChart()
        {
            var data = await _dashboardService.GetTopProductsChartAsync();
            return Ok(new { Success = true, Data = data });
        }

        [HttpGet("bottom-products")]
        public async Task<IActionResult> GetBottomProductsChart()
        {
            var data = await _dashboardService.GetBottomProductsChartAsync();
            return Ok(new { Success = true, Data = data });
        }

        [HttpGet("chart-data")]
        public async Task<IActionResult> GetChartData()
        {
            var data = await _dashboardService.GetChartDataAsync();
            return Ok(new { Success = true, Data = data });
        }
    }
}
