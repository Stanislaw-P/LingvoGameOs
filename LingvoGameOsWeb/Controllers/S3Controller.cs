using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class S3Controller : Controller
    {
        readonly S3Service _s3Service;
        readonly UserManager<User> _userManager;

        public S3Controller(S3Service s3Service, UserManager<User> userManager)
        {
            _s3Service = s3Service;
            _userManager = userManager;
        }

        public IActionResult Index(string key)
        {
            try
            {
                ViewBag.FileUrl = _s3Service.GetPublicUrl(key);
                return View();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                string userId = Guid.NewGuid().ToString();
                // Используем наш сервис для загрузки
                var result = await _s3Service.UploadAvatarFileAsync(file, userId, Folders.Avatars);

                return Ok(new
                {
                    Message = "Файл успешно загружен",
                    Key = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при загрузке: {ex.Message}");
            }
        }

        public async Task<IActionResult> DeleteImage(string key)
        {
            await _s3Service.DeleteFileAsync(key);
            return Ok();
        }
    }
}
