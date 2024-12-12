using Microsoft.AspNetCore.Mvc;
using QuickMart.Services.Services.IServices;
using QuickMart.Services.DTO;
using System.Threading.Tasks;
using QuickMart.Services.Helpers;
using QuickMart.Data.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

        //

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDTO">The user data to be registered.</param>
        /// <returns>Returns the created user data.</returns>
        /// <response code="201">Returns if the user was successfully registered.</response>
        /// <response code="400">If the user data is invalid.</response>
        /// <response code="500">If an error occurs while registering the user.</response>
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUserDTO userDTO)
        {
            if (userDTO == null || string.IsNullOrEmpty(userDTO.Email) || string.IsNullOrEmpty(userDTO.PasswordHash))
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                var createdUser = await userService.CreateUserAsync(userDTO, userDTO.Role ?? "User");
                return CreatedAtAction(nameof(CreateUser), createdUser);
            }
            catch (Exception ex)
            {
                // Check if the exception is about email already existing
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
        /// <param name="loginDTO">The login credentials (email and password).</param>
        /// <returns>Returns the JWT token for the authenticated user.</returns>
        /// <response code="200">Returns the JWT token if authentication is successful.</response>
        /// <response code="401">If the credentials are invalid.</response>
        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateUser([FromBody] LoginDTO loginDTO)
        {
            var user = await userService.AuthenticateUserAsync(loginDTO.Email, loginDTO.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = jwtHelper.GenerateToken(user);  // Ensure you have this method for generating JWT
            return Ok(new { Token = token });
        }



        /// <summary>
        /// Retrieves the logged-in user's information.
        /// </summary>
        /// <returns>Returns the user's data if authorized.</returns>
        /// <response code="200">Returns the user data.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If an error occurs while fetching the user information.</response>
        [HttpGet("ByUserid")]
        [Authorize]
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
        /// <returns>Returns a list of users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If an error occurs while fetching the users.</response>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            try
            {
                var users = await userService.GetAllUsersAsync(false); // false: exclude soft-deleted users
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
        /// <returns>Returns a list of soft-deleted users.</returns>
        /// <response code="200">Returns the list of soft-deleted users.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If an error occurs while fetching the soft-deleted users.</response>
        [HttpGet("softdeleted")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetSoftDeletedUsers()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

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



        /// <summary>
        /// Updates user data by user ID.
        /// </summary>
        /// <param name="id">The user ID to update.</param>
        /// <param name="userDTO">The user data to be updated.</param>
        /// <returns>Returns the updated user data.</returns>
        /// <response code="200">Returns the updated user data.</response>
        /// <response code="400">If the user data is invalid.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an error occurs while updating the user.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUserDTO userDTO)
        {
            if (userDTO == null || string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid user data.");
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
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
        /// <param name="id">The user ID to be soft deleted.</param>
        /// <returns>Returns a message if the user is successfully soft-deleted.</returns>
        /// <response code="200">If the user was soft-deleted successfully.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an error occurs while deleting the user.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

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



        /// <summary>
        /// Creates a new role (Admin role required).
        /// </summary>
        /// <param name="roleName">The name of the role to be created.</param>
        /// <returns>Returns the created role.</returns>
        /// <response code="201">If the role was successfully created.</response>
        /// <response code="400">If the role name is invalid.</response>
        /// <response code="500">If an error occurs while creating the role.</response>
        [HttpPost("CreateRole")]
        [Authorize(Roles = "Admin")]
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



        /// <summary>
        /// Sends a password reset link to the specified email address.
        /// </summary>
        /// <param name="forgotPasswordDTO">The email address to send the reset link.</param>
        /// <returns>Returns a message indicating if the reset link has been sent.</returns>
        /// <response code="200">If the reset link was sent successfully.</response>
        /// <response code="400">If the email is invalid.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an error occurs while sending the reset link.</response>
        [HttpPost("forgotpassword")]
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
        /// <param name="resetPasswordDTO">The reset password data (email, token, new password).</param>
        /// <returns>Returns a message indicating if the password was successfully reset.</returns>
        /// <response code="200">If the password was reset successfully.</response>
        /// <response code="400">If the data is invalid or the token is incorrect.</response>
        /// <response code="500">If an error occurs while resetting the password.</response>
        [HttpPost("resetpassword")]
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
    }
}
