using LibraryHub.Common.Dtos;

namespace LibraryHub.Api.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetAsync();
    }
}