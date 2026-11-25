using System.ComponentModel.DataAnnotations;

namespace LibraryHub.Web.ViewModels
{
    public class MemberFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string? Phone { get; set; }

        [Display(Name = "Active member")]
        public bool IsActive { get; set; } = true;
    }
}