using LibraryHub.Api.Services;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Api.Controllers
{
    [Route("api/[controller]")]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dashboard = await _dashboardService.GetAsync();
            return Ok(ApiResponse<DashboardDto>.Ok(dashboard));
        }
    }
}