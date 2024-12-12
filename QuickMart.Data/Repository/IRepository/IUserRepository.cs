using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using QuickMart.Data.Entities;
using QuickMart.Services.DTO;

namespace QuickMart.Data.Repository.IRepository
{
    public interface IUserRepository
    {
        #region User Management
        Task<ApplicationUserDTO> CreateUserAsync(ApplicationUserDTO userDTO, string roleName); // Create user and return DTO
        Task<ApplicationUserDTO> GetUserByUserIdAsync(string userId); // Fetch user by ID
        Task<IEnumerable<ApplicationUserDTO>> GetAllUsersAsync(bool excludeSoftDeleted); // Get all users, filter soft deleted
        Task<ApplicationUserDTO> UpdateUserAsync(string id, ApplicationUserDTO userDTO); // Update user
        Task<bool> SoftDeleteUserAsync(string id); // Soft delete user
        Task<IEnumerable<ApplicationUserDTO>> GetSoftDeletedUsersAsync(); // Fetch soft-deleted users
        #endregion

        #region Role Management
        Task<IdentityRole> CreateRoleAsync(string roleName); // Create new role
        Task<bool> AssignRoleToUserByIdAsync(string userId, string roleName); // Assign role to user by ID
        #endregion

        #region Authentication & Password Management
        Task<ApplicationUser> FindUserByEmailAsync(string email); // Find user by email
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user); // Generate password reset token
        Task<bool> ResetPasswordAsync(ApplicationUser user, string token, string newPassword); // Reset user password
        #endregion
    }
}
