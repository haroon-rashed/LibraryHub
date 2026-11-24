using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LibraryHub.Common.Dtos
{
    public class SaveBookDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Isbn { get; set; } = string.Empty;

        [Range(1000, 2100, ErrorMessage = "Enter a valid published year.")]
        public int PublishedYear { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1.")]
        public int TotalCopies { get; set; }
    }
}