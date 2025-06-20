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

        public async Task<List<string>> SafeImagesFilesAsync(IFormFile[] files, ImageFolders folder)
        {
            var imagePaths = new List<string>();
            foreach (var file in files)
            {
                var imagePath = await SafeFileAsync(file, folder);
                if(imagePath != null)
                    imagePaths.Add(imagePath);
            }
            return imagePaths;
        }

        // Для картинок
        public async Task<string?> SafeFileAsync(IFormFile file, ImageFolders folder)
        {
            if (file == null)
                return null;

            var folderPath = Path.Combine(_appEnvironment.WebRootPath + "/img/" + folder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + "." + file.FileName.Split('.').Last();
            string path = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/img/" + folder + "/" + fileName;
        }

        // Для файлов игр
        public async Task<string?> SafeFileAsync(IFormFile uploadedFile, int gameId, string gameTitle)
        {
            if (uploadedFile == null)
                return null;
            string folderPath = Path.Combine(_appEnvironment.WebRootPath + "/games/" + gameId);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = gameTitle + "." + uploadedFile.FileName.Split('.').Last();
            string path = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }

            return "/games/" + gameId + "/" + fileName;
        }
    }
}
