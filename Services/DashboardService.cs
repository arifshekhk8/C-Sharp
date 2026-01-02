using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalBooks = await _context.Books.CountAsync();

            var newlyArrivedBooks = await _context.Books
                .CountAsync(b => b.BookCategory == BookCategory.NewlyArrived);

            var mostPopularBooks = await _context.Books
                .CountAsync(b => b.BookCategory == BookCategory.MostPopular);

            var totalUsers = await _context.Users.CountAsync();

            var activeLoans = await _context.Loans
                .CountAsync(l => l.Status == "Approved");


                
            return new AdminDashboardStatsDto
            {
                TotalBooks = totalBooks,
                NewlyArrivedBooks = newlyArrivedBooks,
                MostPopularBooks = mostPopularBooks,
                TotalUsers = totalUsers,
                ActiveLoans = activeLoans,
            };
        }
    }
}