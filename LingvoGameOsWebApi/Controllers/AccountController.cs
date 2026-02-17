using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOsWebApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> LoginAsync([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Email and password are required");

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    return Unauthorized("Invalid login or password");

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                    return Ok(new AuthResponse(user.Id));

                return Unauthorized("Invalid login or password");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string userId);
}
