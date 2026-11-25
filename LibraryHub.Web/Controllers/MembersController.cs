using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using LibraryHub.Web.Extensions;
using LibraryHub.Web.Services;
using LibraryHub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Web.Controllers
{
    public class MembersController : Controller
    {
        private const int PageSize = 5;

        private readonly LibraryApiClient _api;

        public MembersController(LibraryApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            var response = await _api.GetMembersAsync(search, page, PageSize);

            var model = new MemberListViewModel
            {
                Search = search,
                Result = response.Data ?? new PagedResult<MemberDto>()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Form", new MemberFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var response = await _api.CreateMemberAsync(ToDto(model));

            if (!response.Success)
            {
                ModelState.AddApiErrors(response);
                return View("Form", model);
            }

            TempData["Success"] = "Member created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _api.GetMemberAsync(id);

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var member = response.Data;

            var model = new MemberFormViewModel
            {
                Id = member.Id,
                FullName = member.FullName,
                Email = member.Email,
                Phone = member.Phone,
                IsActive = member.IsActive
            };

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberFormViewModel model)
        {
            model.Id = id;

            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var response = await _api.UpdateMemberAsync(id, ToDto(model));

            if (!response.Success)
            {
                ModelState.AddApiErrors(response);
                return View("Form", model);
            }

            TempData["Success"] = "Member updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _api.DeleteMemberAsync(id);

            if (response.Success)
            {
                TempData["Success"] = "Member deleted successfully.";
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static SaveMemberDto ToDto(MemberFormViewModel model)
        {
            return new SaveMemberDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                IsActive = model.IsActive
            };
        }
    }
}