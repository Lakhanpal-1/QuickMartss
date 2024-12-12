using Microsoft.AspNetCore.Identity;
using QuickMart.Data.Entities;
using QuickMart.Data.Repository.IRepository;
using QuickMart.Services.DTO;
using QuickMart.Services.Services.IServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuickMart.Data.Repository;
using System;

namespace QuickMart.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailService emailService;

        // Constructor to inject the UserRepository, UserManager, and EmailService
        public UserService(IUserRepository _userRepository, UserManager<ApplicationUser> _userManager, IEmailService _emailService)
        {
            userRepository = _userRepository;
            userManager = _userManager;
            emailService = _emailService;
        }

        // 1. Create User (Registration)
        public async Task<ApplicationUserDTO> CreateUserAsync(ApplicationUserDTO userDTO, string roleName)
        {
            return await userRepository.CreateUserAsync(userDTO, roleName);
        }

        // 2. Authenticate User (Login)
        public async Task<ApplicationUser> AuthenticateUserAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null && await userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }

        // 3. Get User by User ID
        public async Task<ApplicationUserDTO> GetUserByUserIdAsync(string userId)
        {
            return await userRepository.GetUserByUserIdAsync(userId);
        }

        // 4. Get All Users (Admin/Manager)
        public async Task<IEnumerable<ApplicationUserDTO>> GetAllUsersAsync(bool excludeSoftDeleted)
        {
            return await userRepository.GetAllUsersAsync(excludeSoftDeleted); // Pass the parameter to the repository method
        }

        // 5. Update User
        public async Task<ApplicationUserDTO> UpdateUserAsync(string id, ApplicationUserDTO userDTO)
        {
            return await userRepository.UpdateUserAsync(id, userDTO);
        }

        // 6. Soft Delete User
        public async Task<bool> SoftDeleteUserAsync(string id)
        {
            return await userRepository.SoftDeleteUserAsync(id);
        }

        // 7. Get Soft Deleted Users (Admin/Manager)
        public async Task<IEnumerable<ApplicationUserDTO>> GetSoftDeletedUsersAsync()
        {
            return await userRepository.GetSoftDeletedUsersAsync(); // Fetch soft-deleted users
        }

        // 8. Create Role (Admin only)
        public async Task<IdentityRole> CreateRoleAsync(string roleName)
        {
            return await userRepository.CreateRoleAsync(roleName);
        }

        // 9. Assign Role to User (Admin only)
        public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
        {
            return await userRepository.AssignRoleToUserByIdAsync(userId, roleName);
        }

        // 10. Generate Password Reset Token (Forgot Password)
        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await userRepository.FindUserByEmailAsync(email);
            if (user == null)
            {
                return null; // User not found
            }

            var token = await userRepository.GeneratePasswordResetTokenAsync(user);

            // Send email with the reset token
            var resetLink = $"https://your-app.com/reset-password?token={token}&email={email}";
            var emailBody = $"Click the link to reset your password: <a href='{resetLink}'>{resetLink}</a>";

            // Send email using the email service
            await emailService.SendEmailAsync(email, "Password Reset", emailBody);

            return token; // Optional: return the token for debugging
        }

        // 11. Reset Password
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await userRepository.FindUserByEmailAsync(email);
            if (user == null)
            {
                return false; // User not found
            }

            return await userRepository.ResetPasswordAsync(user, token, newPassword);
        }
    }
}
