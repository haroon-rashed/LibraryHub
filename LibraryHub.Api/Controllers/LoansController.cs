using LibraryHub.Api.Services;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Api.Controllers
{
    [Route("api/[controller]")]
    public class LoansController : ApiControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string status = "all")
        {
            var result = await _loanService.GetAllAsync(status);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return Ok(ApiResponse<List<LoanDto>>.Ok(result.Data!));
        }

        [HttpPost]
        public async Task<IActionResult> Borrow(CreateLoanDto dto)
        {
            var result = await _loanService.BorrowAsync(dto);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            var message = $"Book borrowed successfully. Due on {result.Data!.DueOn:dd MMM yyyy}.";
            return StatusCode(StatusCodes.Status201Created, ApiResponse<LoanDto>.Ok(result.Data, message));
        }

        [HttpPost("{id:int}/return")]
        public async Task<IActionResult> Return(int id)
        {
            var result = await _loanService.ReturnAsync(id);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            var message = result.Data!.LateFee > 0
                ? $"Book returned. Late fee: {result.Data.LateFee:0.##}."
                : "Book returned on time.";

            return Ok(ApiResponse<LoanDto>.Ok(result.Data, message));
        }
    }
}