using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;

namespace LibraryHub.Web.ViewModels
{
    public class MemberListViewModel
    {
        public string? Search { get; set; }
        public PagedResult<MemberDto> Result { get; set; } = new();
    }
}