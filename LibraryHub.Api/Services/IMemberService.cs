using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;

namespace LibraryHub.Api.Services
{
    public interface IMemberService
    {
        Task<PagedResult<MemberDto>> GetAllAsync(string? search, bool activeOnly, int page, int pageSize);
        Task<ServiceResult<MemberDto>> GetByIdAsync(int id);
        Task<ServiceResult<MemberDto>> CreateAsync(SaveMemberDto dto);
        Task<ServiceResult<MemberDto>> UpdateAsync(int id, SaveMemberDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}