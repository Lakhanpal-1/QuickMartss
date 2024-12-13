using Microsoft.AspNetCore.Mvc;
using QuickMart.Services.Services.IServices;
using QuickMart.Services.DTO;
using System.Threading.Tasks;
using QuickMart.Services.Helpers;
using QuickMart.Data.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Identity;

namespace QuickMart.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly JwtHelper jwtHelper;

        public UserController(IUserService _userService, JwtHelper _jwtHelper)
        {
            userService = _userService;
            jwtHelper = _jwtHelper;
        }

        #region User Registration and Authentication

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        [SwaggerResponse(201, "User successfully created.", typeof(ApplicationUserDTO))]
        [SwaggerResponse(400, "Invalid user data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdUser = await userService.CreateUserAsync(userDTO, userDTO.Role);
                return CreatedAtAction(nameof(CreateUser), createdUser);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        [HttpPost("login")]
        [SwaggerResponse(200, "User authenticated successfully.", typeof(object))]
        [SwaggerResponse(401, "Invalid credentials.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> AuthenticateUser([FromBody] LoginDTO loginDTO)
        {
            var user = await userService.AuthenticateUserAsync(loginDTO.Email, loginDTO.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = jwtHelper.GenerateToken(user);
            return Ok(new { Token = token });
        }

        #endregion

        #region User Information Retrieval

        /// <summary>
        /// Retrieves the logged-in user's information.
        /// </summary>
        [HttpGet("ByUserid")]
        [Authorize]
        [SwaggerResponse(200, "User information retrieved successfully.", typeof(ApplicationUserDTO))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(404, "User not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetUserByUserId()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            try
            {
                var user = await userService.GetUserByUserIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all users (Admin/Manager role required).
        /// </summary>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerResponse(200, "List of users retrieved successfully.", typeof(IEnumerable<ApplicationUserDTO>))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await userService.GetAllUsersAsync(false);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves soft-deleted users (Admin/Manager role required).
        /// </summary>
        [HttpGet("softdeleted")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerResponse(200, "List of soft-deleted users retrieved successfully.", typeof(IEnumerable<ApplicationUserDTO>))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetSoftDeletedUsers()
        {
            try
            {
                var users = await userService.GetSoftDeletedUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region User Updates and Deletions

        /// <summary>
        /// Updates user data by user ID.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        [SwaggerResponse(200, "User updated successfully.", typeof(ApplicationUserDTO))]
        [SwaggerResponse(400, "Invalid user data.")]
        [SwaggerResponse(404, "User not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUserDTO userDTO)
        {
            if (userDTO == null || string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                var updatedUser = await userService.UpdateUserAsync(id, userDTO);
                if (updatedUser == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Soft deletes a user by ID (Admin role required).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerResponse(200, "User soft deleted successfully.")]
        [SwaggerResponse(404, "User not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            try
            {
                var result = await userService.SoftDeleteUserAsync(id);
                if (!result)
                {
                    return NotFound("User not found.");
                }

                return Ok("User soft deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Role Management

        /// <summary>
        /// Creates a new role (Admin role required).
        /// </summary>
        [HttpPost("CreateRole")]
        [SwaggerResponse(201, "Role created successfully.", typeof(IdentityRole))]
        [SwaggerResponse(400, "Invalid role name.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            try
            {
                var createdRole = await userService.CreateRoleAsync(roleName);
                return CreatedAtAction(nameof(CreateRole), new { roleName = createdRole.Name });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Password Reset

        /// <summary>
        /// Sends a password reset link to the specified email address.
        /// </summary>
        [HttpPost("forgotpassword")]
        [SwaggerResponse(200, "Password reset link sent successfully.")]
        [SwaggerResponse(400, "Email is required.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            if (string.IsNullOrEmpty(forgotPasswordDTO.Email))
            {
                return BadRequest("Email is required.");
            }

            try
            {
                var token = await userService.GeneratePasswordResetTokenAsync(forgotPasswordDTO.Email);
                if (string.IsNullOrEmpty(token))
                {
                    return NotFound("User not found.");
                }

                return Ok(new { Message = "If the email exists, a password reset link has been sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the password using the specified token.
        /// </summary>
        [HttpPost("resetpassword")]
        [SwaggerResponse(200, "Password has been reset successfully.")]
        [SwaggerResponse(400, "Invalid token or email.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (string.IsNullOrEmpty(resetPasswordDTO.Token) || string.IsNullOrEmpty(resetPasswordDTO.Email) || string.IsNullOrEmpty(resetPasswordDTO.NewPassword))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                var result = await userService.ResetPasswordAsync(resetPasswordDTO.Email, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);
                if (!result)
                {
                    return BadRequest("Invalid token or email.");
                }

                return Ok(new { Message = "Password has been reset successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}
