using LibraryHub.Common.Dtos;

namespace LibraryHub.Web.ViewModels
{
    public class LoanListViewModel
    {
        public string Status { get; set; } = "all";
        public List<LoanDto> Loans { get; set; } = new();
    }
}