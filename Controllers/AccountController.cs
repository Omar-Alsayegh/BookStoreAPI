using Azure.Core;
using BookStoreApi.Controllers;
using BookStoreApi.Models;
using BookStoreApi.Models.DTOs.Auth;
using BookStoreApi.Models.DTOs.Response;
using BookStoreApi.Services;
using BookStoreApi.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BookStoreApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<ApplicationUser> userManager, ITokenService tokenService, ILogger<AccountController> logger, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
        }

        //[HttpPost("register")]

        //public async Task<IActionResult> Result([FromBody] RegisterDto registerDto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        var appuser = new ApplicationUser
        //        {
        //            UserName = registerDto.Username,
        //            Email = registerDto.EmailAddress
        //        };

        //        var createduser = await _userManager.CreateAsync(appuser, registerDto.Password);
        //        if (createduser.Succeeded) {
        //            var roleResult = await _userManager.AddToRoleAsync(appuser, "User");
        //            if (roleResult.Succeeded)
        //            {
        //                return Ok("User Created");
        //            }

        //            else
        //            {
        //                return StatusCode(500, roleResult.Errors);
        //            }
        //            }
        //        else { 
        //            return StatusCode(500, createduser.Errors);
        //            }

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode (500, ex.Message);
        //    }

        //}

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            _logger.LogInformation("Attempting to register user: {Email}", request.EmailAddress);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Register attempt for {Email} failed due to invalid model state.", request.EmailAddress);
                return BadRequest(ModelState);
            }
            var userExists = await _userManager.FindByEmailAsync(request.EmailAddress);
            if (userExists != null && userExists.NormalizedEmail==request.EmailAddress.Normalize())
            {
                _logger.LogWarning("Register attempt for {Email} failed: User already exists.", request.EmailAddress);
                return BadRequest("User with this email already exists.");
            }

            var newUser = new ApplicationUser
            {
                Email = request.EmailAddress,
                UserName = request.EmailAddress, // Often set UserName to Email
                FirstName = request.FirstName, // Optional: if you added these to ApplicationUser
                LastName = request.LastName,   // Optional
                DateJoined = DateTime.UtcNow // Optional
            };
            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (result.Succeeded)
            {
                if (!await _userManager.IsInRoleAsync(newUser, "Customer")) 
                {
                    await _userManager.AddToRoleAsync(newUser, "Customer");
                    _logger.LogInformation("User {Email} assigned 'Customer' role.", request.EmailAddress);
                }
                _logger.LogInformation("User {Email} registered successfully.", request.EmailAddress);
                return Ok("User registered successfully.");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Error registering user {Email}: {Code} - {Description}", request.EmailAddress, error.Code, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("Attempting to log in user: {Email}", request.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login attempt for {Email} failed due to invalid model state.", request.Email);
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt for {Email} failed: User not found.", request.Email);
                return Unauthorized("Invalid credentials.");
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordCheck)
            {

                await _userManager.AccessFailedAsync(user);
                _logger.LogWarning("Login attempt for {Email} failed: Incorrect password.", request.Email);
                return Unauthorized("Invalid credentials.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenService.CreateJwtToken(user, roles.ToList());
            _logger.LogInformation("User {Email} logged in successfully.", request.Email);
            return Ok(new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                JwtToken = jwtToken,
                Roles = roles.ToList()
            });
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Message = "Account is not found!" });
            }
            var coolDownMinutes = _configuration.GetValue<int>("PasswordResetSettings:CoolDownMinutes", 5); 
            if (user.LastPasswordResetRequestUtc.HasValue &&
                (DateTime.UtcNow - user.LastPasswordResetRequestUtc.Value).TotalMinutes < coolDownMinutes)
            {
                _logger.LogInformation("Password reset request for {Email} blocked due to cool-down. Last request was {LastRequestUtc} UTC.",
                    request.Email, user.LastPasswordResetRequestUtc.Value);
                return StatusCode(StatusCodes.Status429TooManyRequests,
            new { Message = $"You have requested a password reset recently. Please wait {coolDownMinutes} minutes before trying again." });
            }

           

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetUrl = $"{Request.Scheme}://{Request.Host}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

            user.LastPasswordResetRequestUtc = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var emailSubject = "Reset Your Password for BookStore API";
            var tokenExpirationHours = _userManager.Options.Tokens.PasswordResetTokenProvider == "Default" ? 1 : // Default is 1 hour
                                  _userManager.Options.Tokens.PasswordResetTokenProvider == "DataProtectorTokenProvider" ? 1 : // DataProtector also default 1 hour
                                                                                                                               // Add specific logic if you use custom token providers
                                  1;
            var emailMessage = $"<!DOCTYPE html><html><head><title>Password Reset</title></head><body>" +
                           $"<p>Dear {user.UserName ?? user.Email},</p>" +
                           $"<p>You are receiving this email because we received a password reset request for your account. If you did not request a password reset, please ignore this email.</p>" +
                           $"<p>To reset your password, please click on the link below:</p>" +
                           $"<p><a href=\"{resetUrl}\">Reset My Password</a></p>" +
                           $"<p>This link will expire in **{tokenExpirationHours} hour(s)** for security reasons.</p>" + // <--- NEW: Expiry info
                           $"<p>If the link above does not work, copy and paste the following URL into your browser:</p>" +
                           $"<p>{resetUrl}</p>" +
                           $"<br>" +
                           $"<p>Thanks,</p>" +
                           $"<p>The BookStore API Team</p>" +
                           $"</body></html>";


            try
            {
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { Message = "Error sending email.", Details = ex.Message });
            }
            return Ok(new { Message = "If an account exists for this email, a password reset link has been sent." });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid password reset request." });
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password has been reset successfully." });
            }
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Message = "Password reset failed.", Errors = errors });
        }

        [HttpPost("admin/assign-role")]
        [Authorize(Roles = "Admin")] // Only users with the "Admin" role can access this
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // User not found
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Access denied due to roles
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignRole([FromBody] ManageUserRoleRequestDto request)
        {
            // Log who is attempting the action (optional, but good for auditing)
            var currentAdminEmail = User.Identity?.Name;
            _logger.LogInformation("Admin '{AdminEmail}' attempting to assign role '{RoleName}' to user '{UserIdentifier}'.",
                                    currentAdminEmail, request.RoleName, request.UserIdentifier);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Assign role attempt for user '{UserIdentifier}' failed due to invalid model state.", request.UserIdentifier);
                return BadRequest(ModelState);
            }

            // Find the user by email or username
            // We try ByEmail first, then ByName as UserName is often set to Email.
            var user = await _userManager.FindByEmailAsync(request.UserIdentifier) ?? await _userManager.FindByNameAsync(request.UserIdentifier);
            if (user == null)
            {
                _logger.LogWarning("Assign role attempt for user '{UserIdentifier}' failed: User not found.", request.UserIdentifier);
                return NotFound("User not found.");
            }

            // --- Role Name Validation ---
            // Ensure the role name being assigned is one of your predefined roles.
            // An Admin should be able to assign Admin, Employee, or Customer roles.
            string[] allowedRoles = { "Admin", "Employee", "Customer" };
            if (!Array.Exists(allowedRoles, role => role.Equals(request.RoleName, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(request.RoleName), $"Invalid role name. Allowed roles are: {string.Join(", ", allowedRoles)}.");
                _logger.LogWarning("Admin '{AdminEmail}' failed to assign role: Invalid role name '{RoleName}'.", currentAdminEmail, request.RoleName);
                return BadRequest(ModelState);
            }

            // --- Prevent assigning role if user already has it ---
            if (await _userManager.IsInRoleAsync(user, request.RoleName))
            {
                _logger.LogInformation("User '{UserIdentifier}' already has role '{RoleName}'. No change needed.", request.UserIdentifier, request.RoleName);
                return Ok($"User '{request.UserIdentifier}' already has role '{request.RoleName}'.");
            }

            // --- Add the role to the user ---
            var result = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (result.Succeeded)
            {
                _logger.LogInformation("User '{UserIdentifier}' successfully assigned role '{RoleName}' by Admin '{AdminEmail}'.",
                                        request.UserIdentifier, request.RoleName, currentAdminEmail);
                return Ok($"Role '{request.RoleName}' assigned to user '{request.UserIdentifier}' successfully.");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Error assigning role '{RoleName}' to user '{UserIdentifier}': {Code} - {Description}",
                                 request.RoleName, request.UserIdentifier, error.Code, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("admin/remove-role")] // Using POST for state change, could also be DELETE if body is allowed
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // User not found
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Not an Admin
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveRole([FromBody] ManageUserRoleRequestDto request) // Reusing the DTO
        {
            var currentAdminEmail = User.Identity?.Name;
            _logger.LogInformation("Admin '{AdminEmail}' attempting to remove role '{RoleName}' from user '{UserIdentifier}'.",
                                    currentAdminEmail, request.RoleName, request.UserIdentifier);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Remove role attempt for user '{UserIdentifier}' failed due to invalid model state.", request.UserIdentifier);
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(request.UserIdentifier) ?? await _userManager.FindByNameAsync(request.UserIdentifier);
            if (user == null)
            {
                _logger.LogWarning("Remove role attempt for user '{UserIdentifier}' failed: User not found.", request.UserIdentifier);
                return NotFound("User not found.");
            }

            // --- Role Name Validation ---
            string[] allowedRoles = { "Admin", "Employee", "Customer" };
            if (!Array.Exists(allowedRoles, role => role.Equals(request.RoleName, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(request.RoleName), $"Invalid role name. Allowed roles are: {string.Join(", ", allowedRoles)}.");
                _logger.LogWarning("Admin '{AdminEmail}' failed to remove role: Invalid role name '{RoleName}'.", currentAdminEmail, request.RoleName);
                return BadRequest(ModelState);
            }

            // --- Prevent removing role if user doesn't have it ---
            if (!await _userManager.IsInRoleAsync(user, request.RoleName))
            {
                _logger.LogInformation("User '{UserIdentifier}' does not have role '{RoleName}'. No change needed.", request.UserIdentifier, request.RoleName);
                return Ok($"User '{request.UserIdentifier}' does not have role '{request.RoleName}'.");
            }

            // --- Remove the role from the user ---
            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role '{RoleName}' successfully removed from user '{UserIdentifier}' by Admin '{AdminEmail}'.",
                                        request.RoleName, request.UserIdentifier, currentAdminEmail);
                return Ok($"Role '{request.RoleName}' removed from user '{request.UserIdentifier}' successfully.");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Error removing role '{RoleName}' from user '{UserIdentifier}': {Code} - {Description}",
                                 request.RoleName, request.UserIdentifier, error.Code, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("CreateUser")]
        [Authorize (Roles ="Admin")]
        public async Task<IActionResult> CreateUser(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Register attempt for {Email} failed due to invalid model state.", registerDto.EmailAddress);
                return BadRequest(ModelState);
            }
            var checkUser = await _userManager.FindByEmailAsync(registerDto.EmailAddress);
            if (checkUser != null)
            {
                _logger.LogWarning("Register attempt failed the user is already found in the database");
                return BadRequest("User already registered");
            }

            var user = new ApplicationUser
            {
                Email = registerDto.EmailAddress,
                UserName= registerDto.EmailAddress,
                FirstName = registerDto?.FirstName,
                LastName = registerDto?.LastName,
                DateJoined= DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                if (!await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    _logger.LogInformation("User {Email} assigned 'Customer' role.", registerDto.EmailAddress);
                }
                _logger.LogInformation("User {Email} registered successfully.", registerDto.EmailAddress);
                return Ok("User registered successfully.");
            }
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error registering user {Email}: {Code} - {Description}", registerDto.EmailAddress, error.Code, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }
    }
}
