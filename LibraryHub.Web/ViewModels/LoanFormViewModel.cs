using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryHub.Web.ViewModels
{
    public class LoanFormViewModel
    {
        [Display(Name = "Book")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a book.")]
        public int BookId { get; set; }

        [Display(Name = "Member")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a member.")]
        public int MemberId { get; set; }

        [ValidateNever]
        public List<SelectListItem> Books { get; set; } = new();

        [ValidateNever]
        public List<SelectListItem> Members { get; set; } = new();
    }
}