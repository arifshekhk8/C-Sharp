namespace LibraryManagementSystem.DTOs
{
    public class AdminDashboardStatsDto
    {
        // Books
        public int TotalBooks { get; set; }
        public int NewlyArrivedBooks { get; set; }
        public int MostPopularBooks { get; set; }

        // Users
        public int TotalUsers { get; set; }

        // Loans
        public int ActiveLoans { get; set; }
        // Revenue
    }
}