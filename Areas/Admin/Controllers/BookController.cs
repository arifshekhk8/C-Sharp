using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            return View(books);
        }

        // ===================== CREATE =====================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            AdminBookFormViewModel model,
            string AuthorName,
            string CategoryName)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AuthorName = AuthorName.Trim();
            CategoryName = CategoryName.Trim();

            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.ToLower() == AuthorName.ToLower());

            if (author == null)
            {
                author = new Author { Name = AuthorName };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == CategoryName.ToLower());

            if (category == null)
            {
                category = new Category { Name = CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            string? pdfPath = await SavePdfAsync(model.PdfFile);

            var book = new Book
            {
                Title = model.Title,
                AuthorId = author.Id,
                BookCategory = model.BookCategory, // NEW enum             
                CategoryId = category.Id,
                Price = model.Price,
                PublicationYear = model.PublicationYear,
                Description = model.Description,
                PdfFilePath = pdfPath
                
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            var model = new AdminBookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author?.Name ?? string.Empty,
                CategoryName = book.Category?.Name ?? string.Empty,
                BookCategory = book.BookCategory, // NEW enum
                Price = book.Price,
                PublicationYear = book.PublicationYear,
                Description = book.Description,
                ExistingPdfFilePath = book.PdfFilePath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            AdminBookFormViewModel model,
            string AuthorName,
            string CategoryName)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var book = await _context.Books.FindAsync(model.Id);
            if (book == null) return NotFound();

            AuthorName = AuthorName.Trim();
            CategoryName = CategoryName.Trim();

            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.ToLower() == AuthorName.ToLower());

            if (author == null)
            {
                author = new Author { Name = AuthorName };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == CategoryName.ToLower());

            if (category == null)
            {
                category = new Category { Name = CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            if (model.PdfFile != null)
            {
                book.PdfFilePath = await SavePdfAsync(model.PdfFile);
            }

            book.Title = model.Title;
            book.AuthorId = author.Id;
            book.BookCategory = model.BookCategory; // NEW enum
            book.CategoryId = category.Id;
            book.Price = model.Price;
            book.PublicationYear = model.PublicationYear;
            book.Description = model.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

                // ===================== DELETE ======================
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            var hasActiveLoans = await _context.Loans
                .AnyAsync(l => l.BookId == id && l.Status == "Active");

            if (hasActiveLoans)
            {
                TempData["Error"] = "Cannot delete this book because it has active loans.";
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            // Delete PDF file if exists
            if (!string.IsNullOrEmpty(book.PdfFilePath))
            {
                var filePath = Path.Combine(_env.WebRootPath, book.PdfFilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== HELPERS =====================
        private async Task<string?> SavePdfAsync(IFormFile? pdf)
        {
            if (pdf == null || pdf.Length == 0) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "pdfs");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(pdf.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await pdf.CopyToAsync(stream);

            return "/pdfs/" + fileName;
        }
    }
}