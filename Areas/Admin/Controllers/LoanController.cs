using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Loan/Manage
        public async Task<IActionResult> Manage()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .OrderByDescending(l => l.Id)
                .ToListAsync();

            return View(loans);
        }

        // POST: /Admin/Loan/Approve/5
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            loan.Status = "Approved";
            loan.ApprovedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }

        // POST: /Admin/Loan/Reject/5
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            loan.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }
    }
}