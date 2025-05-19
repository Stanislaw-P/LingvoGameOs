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
        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                // проблема: всегда запоминает, даже если не нажать галочку
                var result = signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, false).Result;
                if (result.Succeeded)
                {
                    return Redirect(login.ReturnUrl ?? "/Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный пароль");
                }
            }
            return View(login);
        }

        public IActionResult Register(string? returnUrl)
        {
            return View(new RegisterViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (register.UserName == register.Password)
            {
                ModelState.AddModelError("", "Логин и пароль не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                User user = new User() { Email = register.UserName, UserName = register.UserName, Name = register.Name, Surname = register.Surname };
                var result = userManager.CreateAsync(user, register.Password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Constants.PlayerRoleName);
                    signInManager.SignInAsync(user, false).Wait();
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
        public IActionResult Logout()
        {
            signInManager.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
    }
}
