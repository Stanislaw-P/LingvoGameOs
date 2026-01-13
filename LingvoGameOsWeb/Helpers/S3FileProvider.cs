namespace LingvoGameOs.Helpers
{
    public class S3FileProvider
    {
        private readonly S3Service _s3Service;

        public S3FileProvider(S3Service s3Service)
        {
            _s3Service = s3Service;
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
            if (file == null) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            // Формируем ключ: folder/gameId/filename.ext
            var objectKey = $"{folder}/{gameId}/{fileName}";

            await _s3Service.UploadFileAsync(file, objectKey);

            return "/" + objectKey; // Возвращаем путь для БД
        }

        // Для картинок профиля
        public async Task<string?> SaveProfileImgFileAsync(IFormFile file, Folders folder)
        {
            if (file == null) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var objectKey = $"{folder}/{fileName}";

            await _s3Service.UploadFileAsync(file, objectKey);

            return "/" + objectKey;
        }

        // Для файлов игр
        public async Task<string?> SaveGameFileAsync(IFormFile uploadedFile, int gameId, string gameTitle, Folders folder)
        {
            if (uploadedFile == null) return null;

            string fileName = gameTitle + Path.GetExtension(uploadedFile.FileName);
            string objectKey = $"{folder}/{gameId}/{fileName}";

            await _s3Service.UploadFileAsync(uploadedFile, objectKey);

            return "/" + objectKey;
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            // Убираем ведущий слеш, так как Key в S3 обычно начинается сразу с папки
            var objectKey = filePath.TrimStart('/');

            // В S3Service нужно будет добавить метод DeleteFileAsync
            _s3Service.DeleteFileAsync(objectKey).Wait();
        }

        // В S3 нет "директорий", нужно удалять все объекты с определенным префиксом
        public void DeleteDirectory(string folder, int gameId)
        {
            var prefix = $"{folder}/{gameId}/";
            _s3Service.DeleteDirectoryAsync(prefix).Wait();
        }

        // Получение публичной или временной ссылки
        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            return _s3Service.GetFileUrl(filePath.TrimStart('/'));
        }
    }
}
