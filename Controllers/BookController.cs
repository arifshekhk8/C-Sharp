using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Controllers
{
    [Authorize] 
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET:Book
        [AllowAnonymous]
        public IActionResult Index(string search)
        {
            var booksQuery = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                booksQuery = booksQuery.Where(b =>
                    b.Title.Contains(search) ||
                    b.Author.Name.Contains(search));
            }

            var books = booksQuery.ToList();

            var loanedBookIds = _context.Loans
                .Where(l => l.Status == "Approved")
                .Select(l => l.BookId)
                .ToList();

            ViewBag.LoanedBookIds = loanedBookIds;

            return View(books);
        }

        // GET: /Book/Details
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                ViewBag.HasApprovedLoan = _context.Loans.Any(l =>
                    l.BookId == id &&
                    l.UserId == userId &&
                    l.Status == "Approved");
            }
            else
            {
                ViewBag.HasApprovedLoan = false;
            }

            return View(book);
        }

        
    }
}