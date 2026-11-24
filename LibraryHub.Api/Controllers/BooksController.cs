using LibraryHub.Api.Services;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Api.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : ApiControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] bool availableOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.GetAllAsync(search, availableOnly, page, pageSize);
            return Ok(ApiResponse<PagedResult<BookDto>>.Ok(books));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bookService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return Ok(ApiResponse<BookDto>.Ok(result.Data!));
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveBookDto dto)
        {
            var result = await _bookService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                ApiResponse<BookDto>.Ok(result.Data, "Book created."));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SaveBookDto dto)
        {
            var result = await _bookService.UpdateAsync(id, dto);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return Ok(ApiResponse<BookDto>.Ok(result.Data!, "Book updated."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return NoContent();
        }
    }
}