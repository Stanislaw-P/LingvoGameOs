using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class S3Controller : Controller
    {
        readonly S3Service _s3Service;

        public S3Controller(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        public IActionResult Index(string key)
        {
            try
            {
                ViewBag.FileUrl = _s3Service.GetFileUrl(key, 10);
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
            // Генерируем уникальное имя файла, чтобы избежать перезаписи
            var fileExtension = Path.GetExtension(file.FileName);
            var objectKey = $"uploads/{Guid.NewGuid()}{fileExtension}";

            try
            {
                // Используем наш сервис для загрузки
                await _s3Service.UploadFileAsync(file, objectKey);

                return Ok(new
                {
                    Message = "Файл успешно загружен",
                    Key = objectKey
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при загрузке: {ex.Message}");
            }
        }
    }
}
