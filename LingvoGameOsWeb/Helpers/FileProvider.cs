using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Helpers
{
    public partial class FileProvider
    {
        readonly IWebHostEnvironment _appEnvironment;

        public FileProvider(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public async Task<List<string>> SafeImagesFilesAsync(IFormFile[] files, Folders folder, int gameId)
        {
            var imagePaths = new List<string>();
            foreach (var file in files)
            {
                var imagePath = await SafeImgFileAsync(file, folder, gameId);
                if (imagePath != null)
                    imagePaths.Add(imagePath);
            }
            return imagePaths;
        }

        // Для картинок
        public async Task<string?> SafeImgFileAsync(IFormFile file, Folders folder, int gameId)
        {
            if (file == null)
                return null;

            var folderPath = Path.Combine(_appEnvironment.WebRootPath, folder.ToString(), gameId.ToString());
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + "." + file.FileName.Split('.').Last();
            string path = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return Path.Combine("/", folder + "/" + gameId + "/" + fileName);
        }

        // Для файлов игр
        public async Task<string?> SafeGameFileAsync(IFormFile uploadedFile, int gameId, string gameTitle, Folders folder)
        {
            if (uploadedFile == null)
                return null;
            string folderPath = Path.Combine(_appEnvironment.WebRootPath, folder.ToString(), gameId.ToString());
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = gameTitle + "." + uploadedFile.FileName.Split('.').Last();
            string path = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }

            return $"/{folder}/{gameId}/{fileName}";
        }

        public string GetFileFullPath(string filePath)
        {
            try
            {
                return Path.Combine(_appEnvironment.WebRootPath + "/" + filePath);
            }
            catch
            {
                // Тут нужно логировать ошибку
                return "";
            }
        }

        public string GetFileShortPath(string fileName, Folders gameFolder, int gameId)
        {
            try
            {
                return $"/{gameFolder}/{gameId}/{fileName}";
            }
            catch
            {
                // Тут нужно логировать ошибку
                return "";
            }
        }

        public List<ImageFileInfo> GetImagesFilesInfo(List<string> filesPaths)
        {
            return filesPaths
                .Select(filePath => new ImageFileInfo
                {
                    ImagePath = filePath,
                    FileInfo = new FileInfo(Path.Combine(_appEnvironment.WebRootPath + "/" + filePath))
                })
                .ToList();
        }

        public bool MoveGameFolder(string gameId)
        {
            string sourceFolder = Path.Combine(_appEnvironment.WebRootPath, "pendingGame", gameId);
            string destFolder = Path.Combine(_appEnvironment.WebRootPath, "games", gameId);

            try
            {
                // Проверяем существование исходной папки
                if (!Directory.Exists(sourceFolder))
                {
                    Console.WriteLine($"Исходная папка не найдена: {sourceFolder}");
                    return false;
                }

                // Создаем целевую папку, если ее нет
                Directory.CreateDirectory(destFolder);

                // Переносим все файлы
                foreach (string file in Directory.GetFiles(sourceFolder))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destFolder, fileName);

                    File.Move(file, destFile);
                }

                // Удаляем исходную папку после успешного переноса
                Directory.Delete(sourceFolder, true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при переносе папки: {ex.Message}");
                return false;
            }
        }

        private void MoveDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
                File.Move(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (string subDir in Directory.GetDirectories(sourceDir))
                MoveDirectory(subDir, Path.Combine(targetDir, Path.GetFileName(subDir)));

            Directory.Delete(sourceDir);
        }

        public void DeleteFile(string filePath)
        {
            if (filePath == null) return;
            string fullFilePath = Path.Combine(_appEnvironment.WebRootPath + filePath);
            if (File.Exists(fullFilePath))
                File.Delete(fullFilePath);
        }

        public void DeleteImages(List<string> imagesPaths, Folders gameFolder, int gameId)
        {
            if (imagesPaths == null || !imagesPaths.Any()) return;

            foreach (var img in imagesPaths)
            {
                if (!string.IsNullOrEmpty(img))
                {
                    var fullPath = Path.Combine(_appEnvironment.WebRootPath, gameFolder.ToString(), gameId.ToString(), img);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
            }
        }
    }
}
