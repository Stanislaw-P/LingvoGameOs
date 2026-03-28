using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Helpers
{
    public class LocalFileProvider : IFileStorage
    {
        readonly IWebHostEnvironment _appEnvironment;

        public LocalFileProvider(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        // Вспомогательный метод для получения полного физического пути
        private string GetPhysicalPath(string relativePath)
            => Path.Combine(_appEnvironment.WebRootPath, relativePath.TrimStart('/'));

        public async Task<string> UploadGameFileAsync(IFormFile file, int gameId, Folders folder)
        {
            if (file == null) return string.Empty;

            // Создаем путь: /Folders/GameId/Guid.ext
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.Combine(folder.ToString(), gameId.ToString(), fileName).Replace("\\", "/");
            var fullPath = GetPhysicalPath(relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/" + relativePath;
        }

        public async Task<string> UploadAvatarFileAsync(IFormFile file, Folders folder)
        {
            if (file == null) return string.Empty;

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.Combine(folder.ToString(), fileName).Replace("\\", "/");
            var fullPath = GetPhysicalPath(relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/" + relativePath;
        }

        public async Task<List<string>> UploadGameFilesAsync(IFormFile[] files, int gameId, Folders folder)
        {
            var paths = new List<string>();
            foreach (var file in files)
            {
                var path = await UploadGameFileAsync(file, gameId, folder);
                if (!string.IsNullOrEmpty(path)) paths.Add(path);
            }
            return paths;
        }

        public string? GetPublicUrl(string key)
        {
            // Для локального хранилища ключ и есть публичный URL (относительно корня сайта)
            if (string.IsNullOrEmpty(key)) return null;
            return key.StartsWith("/") ? key : "/" + key;
        }

        public string GetDownloadUrl(string key, string displayTitle, string extension)
        {
            // Локально мы просто отдаем тот же URL, браузер обработает скачивание 
            // (или можно добавить логику отдачи через контроллер, но обычно хватает прямой ссылки)
            return GetPublicUrl(key) ?? string.Empty;
        }

        public async Task DeleteFileAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            var fullPath = GetPhysicalPath(key);
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }

        public async Task DeleteDirectoryAsync(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return;
            var fullPath = GetPhysicalPath(prefix);
            if (Directory.Exists(fullPath))
            {
                await Task.Run(() => Directory.Delete(fullPath, true));
            }
        }

        public async Task MoveFileAsync(string sourceKey, string destinationKey)
        {
            var sourcePath = GetPhysicalPath(sourceKey);
            var destPath = GetPhysicalPath(destinationKey);

            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

            await Task.Run(() =>
            {
                if (File.Exists(sourcePath))
                {
                    File.Move(sourcePath, destPath, overwrite: true);
                }
            });
        }

        public async Task<FileMetadata> GetFileMetadataAsync(string key)
        {
            var fullPath = GetPhysicalPath(key);
            if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                return new FileMetadata
                {
                    Key = key,
                    FileUrl = GetPublicUrl(key),
                    Size = fileInfo.Length
                };
            }
            return new FileMetadata();
        }

        public async Task<List<string>> SaveImagesFilesAsync(IFormFile[] files, Folders folder, int gameId)
        {
            var imagePaths = new List<string>();
            foreach (var file in files)
            {
                var imagePath = await SaveGameImgFileAsync(file, folder, gameId);
                if (imagePath != null)
                    imagePaths.Add(imagePath);
            }
            return imagePaths;
        }

        // Для картинок игры
        public async Task<string?> SaveGameImgFileAsync(IFormFile file, Folders folder, int gameId)
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

        // Для картинок профиля
        public async Task<string?> SaveProfileImgFileAsync(IFormFile file, Folders folder)
        {
            if (file == null)
                return null;

            var folderPath = Path.Combine(_appEnvironment.WebRootPath, folder.ToString());
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + "." + file.FileName.Split('.').Last();
            string path = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return Path.Combine("/", folder + "/" + fileName);
        }

        // Для файлов игр
        public async Task<string?> SaveGameFileAsync(IFormFile uploadedFile, int gameId, string gameTitle, Folders folder)
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
            catch (Exception ex)
            {
                // Тут нужно логировать ошибку
                return "";
            }
        }

        public string GetGameFileShortPath(string fileName, Folders gameFolder, int gameId)
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

        //public List<FileMetadata> GetImagesFilesInfo(List<string> filesPaths)
        //{
        //    return filesPaths
        //        .Select(filePath => new FileInfo
        //        {
        //            FileUrl = filePath,
        //            //FileInfo = new FileInfo(Path.Combine(_appEnvironment.WebRootPath + "/" + filePath))
        //        })
        //        .ToList();
        //}

        public string GetGameDirectoryPath(int gameId, Folders folder)
        {
            return Path.Combine(_appEnvironment.WebRootPath, folder.ToString(), gameId.ToString());
        }

        public void MoveGameFiles(int sourceGameId, int destGameId, Folders sourceGameFolder, Folders destGameFolder)
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

        public void MoveGameFile(string sourceGamePath, string destGamePath)
        {
            try
            {
                var sourcePath = Path.Combine(_appEnvironment.WebRootPath + sourceGamePath);
                var destPath = Path.Combine(_appEnvironment.WebRootPath + destGamePath);

                File.Move(sourcePath, destPath);
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

        public string UpdateFileName(string originalPath, string newFileName)
        {
            if (string.IsNullOrEmpty(originalPath))
                return originalPath;
            string oldFileName = originalPath.Split(['/', '\\']).Last();

            return originalPath.Replace(oldFileName, newFileName, StringComparison.OrdinalIgnoreCase);
        }

        public void DeleteFile(string filePath)
        {
            if (filePath == null) return;
            string fullFilePath = Path.Combine(_appEnvironment.WebRootPath + filePath);
            if (File.Exists(fullFilePath))
                File.Delete(fullFilePath);
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
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
