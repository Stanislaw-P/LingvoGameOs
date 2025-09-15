using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LingvoGameOs.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly EmailService emailService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
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
                User user = new User() { Email = register.UserName, UserName = register.UserName, Name = register.Name, Surname = register.Surname, AvatarImgPath = "/img/avatar100.png" };
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

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = model.UserName;
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Эта почта не зарегистрирована");
                    return View(model);
                }
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callBackUrl = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    userEmail = email,
                    userCode = code
                }, protocol: HttpContext.Request.Scheme);
                await emailService.SendEmailAsync(email, "Сброс пароля", $"Для сброса пароля, перейдите <a href='{callBackUrl}'>по ссылке</a>");
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public async Task<IActionResult> ResetPassword(string userCode, string userEmail)
        {
            ViewData["userEmail"] = userEmail;
            return View(new ResetPasswordViewModel { Code = userCode, UserName = userEmail});
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = model.UserName;
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Эта почта не зарегистрирована");
                    return View(model);
                }
                var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Попробуйте сбросить пароль еще раз");
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
