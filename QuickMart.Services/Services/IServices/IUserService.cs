using Microsoft.AspNetCore.Identity;
using QuickMart.Data.Entities;
using QuickMart.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickMart.Services.Services.IServices
{
    public interface IUserService
    {
        // User Management
        Task<ApplicationUserDTO> CreateUserAsync(ApplicationUserDTO userDTO, string roleName);
        Task<ApplicationUserDTO> GetUserByUserIdAsync(string userId);
        Task<IEnumerable<ApplicationUserDTO>> GetAllUsersAsync(bool excludeSoftDeleted);
        Task<ApplicationUserDTO> UpdateUserAsync(string id, ApplicationUserDTO userDTO);
        Task<bool> SoftDeleteUserAsync(string id);
        Task<IEnumerable<ApplicationUserDTO>> GetSoftDeletedUsersAsync();

        // Authentication
        Task<ApplicationUser> AuthenticateUserAsync(string email, string password);

        // Role Management
        Task<bool> AssignRoleToUserAsync(string userId, string roleName);
        Task<IdentityRole> CreateRoleAsync(string roleName);

        // Password Reset
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
