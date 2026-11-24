using LibraryHub.Common.Dtos;

namespace LibraryHub.Api.Services
{
    public interface ILoanService
    {
        Task<ServiceResult<List<LoanDto>>> GetAllAsync(string status);
        Task<ServiceResult<LoanDto>> BorrowAsync(CreateLoanDto dto);
        Task<ServiceResult<LoanDto>> ReturnAsync(int id);
    }
}