using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QuickMart.Data.DbContext;
using QuickMart.Data.Entities;
using QuickMart.Data.Repository.IRepository;
using QuickMart.Services.DTO;
using System.Collections.Generic;

namespace QuickMart.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        // Constructor to inject dependencies
        public UserRepository(ApplicationDbContext _context, IMapper _mapper, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            context = _context;
            mapper = _mapper;
            userManager = _userManager;
            roleManager = _roleManager;
        }

        #region User Management

        public async Task<ApplicationUserDTO> CreateUserAsync(ApplicationUserDTO userDTO, string roleName)
        {
            // If no role is provided, default to "User"
            if (string.IsNullOrEmpty(roleName))
            {
                roleName = "User"; // Default role is "User"
            }

            // Check if the user already exists
            var existingUser = await userManager.FindByEmailAsync(userDTO.Email);
            if (existingUser != null)
            {
                throw new Exception($"A user with the email '{userDTO.Email}' already exists.");
            }

            // Map userDTO to ApplicationUser
            var user = mapper.Map<ApplicationUser>(userDTO);
            user.UserName = userDTO.Email;

            // Create the user
            var result = await userManager.CreateAsync(user, userDTO.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Check if the role exists
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                throw new Exception($"Role '{roleName}' does not exist.");
            }

            // Assign the role to the user
            var roleAssignmentResult = await userManager.AddToRoleAsync(user, roleName);
            if (!roleAssignmentResult.Succeeded)
            {
                throw new Exception($"Role assignment failed: {string.Join(", ", roleAssignmentResult.Errors.Select(e => e.Description))}");
            }

            // Return the user data as DTO
            return mapper.Map<ApplicationUserDTO>(user);
        }


        public async Task<ApplicationUserDTO> GetUserByUserIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user != null ? mapper.Map<ApplicationUserDTO>(user) : null;
        }

        public async Task<IEnumerable<ApplicationUserDTO>> GetAllUsersAsync(bool excludeSoftDeleted)
        {
            var users = userManager.Users.AsQueryable();

            if (excludeSoftDeleted)
            {
                users = users.Where(u => !u.IsDeleted); // Filter out soft-deleted users
            }

            return users.Select(u => mapper.Map<ApplicationUserDTO>(u)).ToList();
        }

        public async Task<ApplicationUserDTO> UpdateUserAsync(string id, ApplicationUserDTO userDTO)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return null;

            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.Email = userDTO.Email;
            user.Address = userDTO.Address;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return mapper.Map<ApplicationUserDTO>(user);
            }

            return null;
        }

        public async Task<bool> SoftDeleteUserAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.IsDeleted = true;  // Mark as deleted (soft delete)
            await userManager.UpdateAsync(user);

            return true;
        }

        public async Task<IEnumerable<ApplicationUserDTO>> GetSoftDeletedUsersAsync()
        {
            var users = userManager.Users.Where(u => u.IsDeleted).ToList();
            return users.Select(u => mapper.Map<ApplicationUserDTO>(u));
        }

        #endregion

        #region Role Management

        public async Task<IdentityRole> CreateRoleAsync(string roleName)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                throw new Exception($"Role '{roleName}' already exists.");
            }

            var role = new IdentityRole(roleName);
            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new Exception($"Role creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return role;
        }

        public async Task<bool> AssignRoleToUserByIdAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                throw new Exception($"Role '{roleName}' does not exist.");
            }

            var result = await userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new Exception($"Role assignment failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return true;
        }

        #endregion

        #region Authentication & Password Management

        public async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        #endregion
    }
}
