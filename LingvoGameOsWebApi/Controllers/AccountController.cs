using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting("login")]
        public async Task<ActionResult<AuthResponse>> LoginAsync([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Email and password are required");

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                // фиктивная проверка пароля для несуществующего email:
                // время ответа одинаково, перечисление email-ов по timing невозможно
                if (user == null)
                {
                    await _signInManager
                        .CheckPasswordSignInAsync(DummyUser, request.Password, lockoutOnFailure: false)
                        .ConfigureAwait(false);
                    return Unauthorized("Invalid login or password");
                }

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

        // Фиктивный пользователь: password hasher всегда выполняет работу, но вход всегда неуспешен
        static readonly User DummyUser = new()
        {
            Id = "dummy-user-id",
            UserName = "dummy@localhost",
            Email = "dummy@localhost",
            SecurityStamp = "dummy-stamp",
            PasswordHash = "AQAAAAIAAYagAAAAEJhF0Vf3I9tG1l1nJ1r7i0m1oR1eQ9O1tG1m1n1oR1eQ9O1tG1m1n1oR1eQ==", // хэш фиктивного пароля
        };
    }

    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string userId);
}
