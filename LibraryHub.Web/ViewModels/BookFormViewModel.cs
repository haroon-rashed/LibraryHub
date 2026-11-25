using System.ComponentModel.DataAnnotations;

namespace LibraryHub.Web.ViewModels
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [MaxLength(200, ErrorMessage = "Author cannot exceed 200 characters.")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required.")]
        [MaxLength(20, ErrorMessage = "ISBN cannot exceed 20 characters.")]
        public string Isbn { get; set; } = string.Empty;

        [Display(Name = "Published year")]
        [Range(1000, 2100, ErrorMessage = "Enter a valid published year.")]
        public int PublishedYear { get; set; } = DateTime.Now.Year;

        [Display(Name = "Total copies")]
        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1.")]
        public int TotalCopies { get; set; } = 1;
    }
}