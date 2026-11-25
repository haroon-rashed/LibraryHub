using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using LibraryHub.Web.Extensions;
using LibraryHub.Web.Services;
using LibraryHub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Web.Controllers
{
    public class BooksController : Controller
    {
        private const int PageSize = 5;

        private readonly LibraryApiClient _api;

        public BooksController(LibraryApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            var response = await _api.GetBooksAsync(search, page, PageSize);

            var model = new BookListViewModel
            {
                Search = search,
                Result = response.Data ?? new PagedResult<BookDto>()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Form", new BookFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var response = await _api.CreateBookAsync(ToDto(model));

            if (!response.Success)
            {
                ModelState.AddApiErrors(response);
                return View("Form", model);
            }

            TempData["Success"] = "Book created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _api.GetBookAsync(id);

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var book = response.Data;

            var model = new BookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Isbn = book.Isbn,
                PublishedYear = book.PublishedYear,
                TotalCopies = book.TotalCopies
            };

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormViewModel model)
        {
            model.Id = id;

            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var response = await _api.UpdateBookAsync(id, ToDto(model));

            if (!response.Success)
            {
                ModelState.AddApiErrors(response);
                return View("Form", model);
            }

            TempData["Success"] = "Book updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _api.DeleteBookAsync(id);

            if (response.Success)
            {
                TempData["Success"] = "Book deleted successfully.";
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static SaveBookDto ToDto(BookFormViewModel model)
        {
            return new SaveBookDto
            {
                Title = model.Title,
                Author = model.Author,
                Isbn = model.Isbn,
                PublishedYear = model.PublishedYear,
                TotalCopies = model.TotalCopies
            };
        }
    }
}