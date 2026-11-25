using LibraryHub.Common.Dtos;
using LibraryHub.Web.Extensions;
using LibraryHub.Web.Services;
using LibraryHub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryHub.Web.Controllers
{
    public class LoansController : Controller
    {
        private readonly LibraryApiClient _api;

        public LoansController(LibraryApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(string status = "all")
        {
            status = status.ToLowerInvariant();

            var response = await _api.GetLoansAsync(status);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
            }

            var model = new LoanListViewModel
            {
                Status = status,
                Loans = response.Data ?? new List<LoanDto>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? bookId, int? memberId)
        {
            var model = new LoanFormViewModel
            {
                BookId = bookId ?? 0,
                MemberId = memberId ?? 0
            };

            await LoadDropdownsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model);
                return View(model);
            }

            var dto = new CreateLoanDto
            {
                BookId = model.BookId,
                MemberId = model.MemberId
            };

            var response = await _api.BorrowAsync(dto);

            if (!response.Success)
            {
                ModelState.AddApiErrors(response);
                await LoadDropdownsAsync(model);
                return View(model);
            }

            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Index), new { status = "active" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, string status = "all")
        {
            var response = await _api.ReturnLoanAsync(id);

            if (response.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return RedirectToAction(nameof(Index), new { status });
        }

        private async Task LoadDropdownsAsync(LoanFormViewModel model)
        {
            var booksResponse = await _api.GetBooksAsync(null, 1, 100, availableOnly: true);
            var membersResponse = await _api.GetMembersAsync(null, 1, 100, activeOnly: true);

            model.Books = (booksResponse.Data?.Items ?? new List<BookDto>())
                .Select(b => new SelectListItem(
                    $"{b.Title} - {b.Author} ({b.AvailableCopies} available)", b.Id.ToString()))
                .ToList();

            model.Members = (membersResponse.Data?.Items ?? new List<MemberDto>())
                .Select(m => new SelectListItem($"{m.FullName} ({m.Email})", m.Id.ToString()))
                .ToList();
        }
    }
}