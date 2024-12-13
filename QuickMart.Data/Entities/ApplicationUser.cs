using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace QuickMart.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

       
        [StringLength(50)]
        public string? Role { get; set; }

        public bool IsDeleted { get; set; }
    }
}
