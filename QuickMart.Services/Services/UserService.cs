using Microsoft.AspNetCore.Identity;
using QuickMart.Data.Entities;
using QuickMart.Data.Repository.IRepository;
using QuickMart.Services.DTO;
using QuickMart.Services.Services.IServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuickMart.Data.Repository;
using System;
using System.Net;

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

        #region User Management

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

        #endregion

        #region Role Management

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

        #endregion

        #region Password Management

        // 10. Generate Password Reset Token (Forgot Password)
        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            // Step 1: Find the user by their email
            var user = await userRepository.FindUserByEmailAsync(email);
            if (user == null)
            {
                // User not found, return null
                return null;
            }

            try
            {
                // Step 2: Generate a password reset token for the user
                var token = await userRepository.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(token))
                {
                    // If the token is null or empty, we can't proceed
                    return null;
                }

                // Step 3: Create the reset password link with the token
                var clientURI = "http://localhost:4200/user/reset-password"; // This should be configurable
                var resetLink = $"{clientURI}?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";

                // Step 4: Prepare the email body with the reset link
                var emailBody = $"Hello {user.FirstName},<br><br>";
                emailBody += "You requested a password reset. Please click the link below to reset your password:<br><br>";
                emailBody += $"<a href='{resetLink}'>Reset Password</a><br><br>";
                emailBody += "If you did not request a password reset, please ignore this email.";

                // Step 5: Send the email using the email service
                await emailService.SendEmailAsync(email, "Password Reset", emailBody);

                return token; // Return the token for debugging/logging purposes
            }
            catch (Exception ex)
            {
                // Log the exception here for debugging or alerting purposes
                Console.WriteLine($"Error generating password reset token: {ex.Message}");
                // Optionally, you can rethrow or handle the error as needed
                throw;
            }
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

        #endregion
    }
}
