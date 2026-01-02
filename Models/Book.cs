namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public BookCategory BookCategory { get; set; }
        
        // Publication year of the book
        public int PublicationYear { get; set; }

        // Book price in BDT
        public decimal Price { get; set; }

        // Stored PDF file path
        public string? PdfFilePath { get; set; }

        // Optional description
        public string? Description { get; set; }

        // Author relationship
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        // Category relationship
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

    }
}