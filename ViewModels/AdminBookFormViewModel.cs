using Microsoft.AspNetCore.Http;
using LibraryManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class AdminBookFormViewModel
    {
        public int Id { get; set; } 

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public BookCategory BookCategory { get; set; }

        public string AuthorName { get; set; }
        public string CategoryName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int PublicationYear { get; set; }

        public string? Description { get; set; }

        // PDF upload (Admin only)
        public IFormFile? PdfFile { get; set; }

        // Existing PDF (for Edit)
        public string? ExistingPdfFilePath { get; set; }
    }
}