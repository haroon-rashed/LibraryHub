using LibraryHub.Common.Dtos;
using LibraryHub.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryApiClient _api;

        public HomeController(LibraryApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _api.GetDashboardAsync();
            return View(response.Data ?? new DashboardDto());
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}