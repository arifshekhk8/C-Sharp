using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public LoanController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Loan/Request/5
        [Authorize]
        [HttpGet]
        public IActionResult Request(int bookId)
        {
            ViewBag.BookId = bookId;
            return View();
        }

        // POST: /Loan/Request/5
        [HttpPost]
        public async Task<IActionResult> Request(int bookId, string address, string phone)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            bool alreadyRequestedByUser = await _context.Loans.AnyAsync(l =>
                l.BookId == bookId &&
                l.UserId == user.Id &&
                (l.Status == "Pending" || l.Status == "Approved"));

            if (alreadyRequestedByUser)
            {
                TempData["Error"] = "You already have a pending or approved request for this book.";
                return RedirectToAction("Details", "Book", new { id = bookId });
            }

            var loan = new Loan
            {
                BookId = bookId,
                UserId = user.Id,
                Address = address,
                Phone = phone,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your loan request has been submitted and is pending admin approval.";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        // GET: /Loan/MyLoans
        public async Task<IActionResult> MyLoans()
        {
            var userId = _userManager.GetUserId(User);

            var loans = await _context.Loans
                .Include(l => l.Book)
                .ThenInclude(b => b.Author)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.RequestDate)
                .ToListAsync();

            return View(loans);
        }

        
        [Authorize]
        public async Task<IActionResult> ReadPdf(int bookId)
        {
            var userId = _userManager.GetUserId(User);

            var loan = await _context.Loans
                .Where(l =>
                    l.BookId == bookId &&
                    l.UserId == userId &&
                    l.Status == "Approved" &&
                    l.ApprovedAt != null)
                .OrderByDescending(l => l.ApprovedAt)
                .FirstOrDefaultAsync();

            if (loan == null)
                return Forbid();

            if (loan.ApprovedAt.Value.AddDays(7) < DateTime.UtcNow)
                return Forbid(); // expired

            var book = await _context.Books.FindAsync(bookId);
            if (book == null || string.IsNullOrEmpty(book.PdfFilePath))
                return NotFound();

            var filePath = Path.Combine(
                _env.WebRootPath,
                book.PdfFilePath.TrimStart('/', '\\'));

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, "application/pdf");
        }
    }
}