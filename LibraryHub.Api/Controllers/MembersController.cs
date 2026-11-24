using LibraryHub.Api.Services;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Api.Controllers
{
    [Route("api/[controller]")]
    public class MembersController : ApiControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] bool activeOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var members = await _memberService.GetAllAsync(search, activeOnly, page, pageSize);
            return Ok(ApiResponse<PagedResult<MemberDto>>.Ok(members));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _memberService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return Ok(ApiResponse<MemberDto>.Ok(result.Data!));
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveMemberDto dto)
        {
            var result = await _memberService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                ApiResponse<MemberDto>.Ok(result.Data, "Member created."));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SaveMemberDto dto)
        {
            var result = await _memberService.UpdateAsync(id, dto);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return Ok(ApiResponse<MemberDto>.Ok(result.Data!, "Member updated."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _memberService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return Failure(result);
            }

            return NoContent();
        }
    }
}