using BookStoreApi.Models;
using BookStoreApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Result([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var appuser = new ApplicationUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.EmailAddress
                };

                var createduser = await _userManager.CreateAsync(appuser, registerDto.Password);
                if (createduser.Succeeded) {
                    var roleResult = await _userManager.AddToRoleAsync(appuser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok("User Created");
                    }

                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                    }
                else { 
                    return StatusCode(500, createduser.Errors);
                    }

            }
            catch (Exception ex)
            {
                return StatusCode (500, ex.Message);
            }

        }
    }
}
