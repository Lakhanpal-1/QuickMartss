using System.ComponentModel.DataAnnotations;

namespace QuickMart.Services.DTO
{
    public class ApplicationUserDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string? PasswordHash { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }
    }
}