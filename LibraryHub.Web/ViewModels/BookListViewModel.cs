using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;

namespace LibraryHub.Web.ViewModels
{
    public class BookListViewModel
    {
        public string? Search { get; set; }
        public PagedResult<BookDto> Result { get; set; } = new();
    }
}