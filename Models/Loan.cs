using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System;


namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public int Id { get; set; }

        // Relationship
        public int BookId { get; set; }
        public Book Book { get; set; }

        // Identity User
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Approved, Rejected

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public DateTime? ApprovedAt { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }
    }
}