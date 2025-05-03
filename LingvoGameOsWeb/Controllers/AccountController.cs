using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                // проблема: всегда запоминает, даже если не нажать галочку
                var result = _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, false).Result;
                if (result.Succeeded)
                {
                    return Redirect(login.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный пароль");
                }
            }
            return View(login);
        }

        public IActionResult Register()
        {
            return View();
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
                return RedirectToAction("Index", "Home");
            }
            return View(register);
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
