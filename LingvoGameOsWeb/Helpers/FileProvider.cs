using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Helpers
{
    public class FileProvider
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

        public async Task MoveGameFilesAsync(int sourceGameId, int destGameId, Folders sourceGameFolder, Folders destGameFolder)
        {
            try
            {
                var sourcePath = Path.Combine(_appEnvironment.WebRootPath, sourceGameFolder.ToString(), sourceGameId.ToString());
                var destPath = Path.Combine(_appEnvironment.WebRootPath, destGameFolder.ToString(), destGameId.ToString());

                if (!Directory.Exists(sourcePath))
                {
                    Console.WriteLine($"Source directory not found: {sourcePath}");
                    return;
                }

                // Создаем целевую директорию, если не существует
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }

                // Копируем все файлы из исходной директории в целевую
                foreach (var file in Directory.GetFiles(sourcePath))
                {
                    var fileName = Path.GetFileName(file);
                    var destFile = Path.Combine(destPath, fileName);
                    File.Move(file, destFile, overwrite: true);
                }

                // Удаляем пустую исходную директорию
                if (Directory.GetFiles(sourcePath).Length == 0)
                {
                    Directory.Delete(sourcePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string UpdateFilePath(string originalPath, string oldBasePath, string newBasePath)
        {
            if (string.IsNullOrEmpty(originalPath))
                return originalPath;

            return originalPath.Replace(oldBasePath, newBasePath, StringComparison.OrdinalIgnoreCase);
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
