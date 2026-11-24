using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;

namespace LibraryHub.Api.Services
{
    public interface IBookService
    {
        Task<PagedResult<BookDto>> GetAllAsync(string? search, bool availableOnly, int page, int pageSize);
        Task<ServiceResult<BookDto>> GetByIdAsync(int id);
        Task<ServiceResult<BookDto>> CreateAsync(SaveBookDto dto);
        Task<ServiceResult<BookDto>> UpdateAsync(int id, SaveBookDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}