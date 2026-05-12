using StitchArtisan.Backend.DTOs;

namespace StitchArtisan.Backend.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
