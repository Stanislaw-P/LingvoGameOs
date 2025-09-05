using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Login(string? returnUrl)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                // проблема: всегда запоминает, даже если не нажать галочку
                var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, false);
                if (result.Succeeded)
                {
                    return Redirect(login.ReturnUrl ?? "/Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин или пароль!");
                }
            }
            return View(login);
        }

        public IActionResult Register(string? returnUrl)
        {
            return View(new RegisterViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (register.UserName == register.Password)
            {
                ModelState.AddModelError("Login == Password", "Логин и пароль не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                User user = new User() { Email = register.UserName, UserName = register.UserName, Name = register.Name, Surname = register.Surname };
                var result = await userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Constants.PlayerRoleName);
                    await signInManager.SignInAsync(user, false);
                    return Redirect(register.ReturnUrl ?? "/Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(register);
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
