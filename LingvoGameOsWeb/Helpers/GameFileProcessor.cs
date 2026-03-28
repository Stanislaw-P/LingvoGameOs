using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db.Helpers;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.Extensions.FileProviders;
using System.Threading.Tasks;

namespace LingvoGameOs.Helpers
{
    public class GameFileProcessor
    {
        readonly IFileStorage _fileStorage;

        public GameFileProcessor(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task ProcessChangeCoverImageAsync(EditGameViewModel editGame, IGameBase existingGame, Folders folder)
        {
            if (editGame.CoverImage != null)
            {
                string? newCoverImagePath = await _fileStorage.UploadGameFileAsync(editGame.CoverImage, existingGame.Id, folder);
                string? oldCoverImagePath = editGame.CurrentCoverImagePath;

                existingGame.CoverImagePath = newCoverImagePath;

                if (newCoverImagePath != oldCoverImagePath)
                    await _fileStorage.DeleteFileAsync(oldCoverImagePath!);
            }
            else
                existingGame.CoverImagePath = editGame.CurrentCoverImagePath!;
        }

        public async Task ProcessDeletedImagesAsync(EditGameViewModel editGameViewModel, IGameBase game)
        {
            if (editGameViewModel.DeletedImages == null || !editGameViewModel.DeletedImages.Any())
                return;

            // Удаляем файлы из хранилища
            foreach (var key in editGameViewModel.DeletedImages)
            {
                await _fileStorage.DeleteFileAsync(key);
            }

            // Оставляем у модели игры в БД только не удаленные картинки
            if (game.ImagesPaths != null)
            {
                var deletedImagesKeys = editGameViewModel.DeletedImages;
                game.ImagesPaths = game.ImagesPaths
                    .Where(imgPath => !deletedImagesKeys.Contains(imgPath))
                    .ToList();
            }
        }

        public async Task ProcessUploadNewImagesAsync(EditGameViewModel editGameViewModel, IGameBase game, Folders folder)
        {
            if (editGameViewModel.UploadedImages == null || !editGameViewModel.UploadedImages.Any(f => f.Length > 0))
                return;

            List<string> newImagesKeys = await _fileStorage.UploadGameFilesAsync(
                editGameViewModel.UploadedImages.Where(f => f.Length > 0).ToArray(),
                editGameViewModel.Id,
                folder);

            if (game.ImagesPaths == null)
                game.ImagesPaths = new List<string>();
            game.ImagesPaths.AddRange(newImagesKeys);
        }

        public async Task ProcessDeleteGameFileAsync(EditGameViewModel editGame, IGameBase existingGame)
        {
            if ((editGame.GamePlatform == "Desktop" && editGame.GameFilePath == null) || (editGame.GamePlatform != "Desktop" && editGame.GameFilePath != null))
            {
                if (editGame.CurrentGameFilePath != null)
                {
                    existingGame.GameFilePath = null;
                    await _fileStorage.DeleteFileAsync(editGame.CurrentGameFilePath);
                }
            }
        }

        public async Task ProcessUploadGameFileAsync(EditGameViewModel editGame, IGameBase existingGame, Folders folder)
        {
            if (editGame.GamePlatform == "Desktop" && editGame.UploadedGameFile != null)
            {
                string newGameFilePath = await _fileStorage.UploadGameFileAsync(editGame.UploadedGameFile, existingGame.Id, folder);
                existingGame.GameFilePath = newGameFilePath;

                if (newGameFilePath != editGame.CurrentGameFilePath)
                    await _fileStorage.DeleteFileAsync(editGame.CurrentGameFilePath!);
            }
        }

        public async Task PublishGameFilesAsync(PendingGame pendingGame, Game publishedGame)
        {
            string sourceFolder = $"{Folders.PendingGames}/{pendingGame.Id}";
            string destFolder = $"{Folders.Games}/{publishedGame.Id}";

            // 1. Перенос обложки (Cover Image)
            if (!string.IsNullOrEmpty(pendingGame.CoverImagePath))
            {
                string newPath = UpdateS3Path(pendingGame.CoverImagePath, sourceFolder, destFolder);
                await _fileStorage.MoveFileAsync(pendingGame.CoverImagePath, newPath);
                publishedGame.CoverImagePath = newPath;
            }

            // 2. Перенос файла игры (если Desktop)
            if (publishedGame?.GamePlatform?.Name == "Desktop" && !string.IsNullOrEmpty(pendingGame.GameFilePath))
            {
                string newPath = UpdateS3Path(pendingGame.GameFilePath, sourceFolder, destFolder);
                await _fileStorage.MoveFileAsync(pendingGame.GameFilePath, newPath);
                publishedGame.GameFilePath = newPath;
            }

            // 3. Перенос скриншотов
            if (pendingGame.ImagesPaths != null && pendingGame.ImagesPaths.Any())
            {
                publishedGame.ImagesPaths = new List<string>();
                foreach (var oldPath in pendingGame.ImagesPaths)
                {
                    string newPath = UpdateS3Path(oldPath, sourceFolder, destFolder);
                    await _fileStorage.MoveFileAsync(oldPath, newPath);
                    publishedGame?.ImagesPaths?.Add(newPath);
                }
            }

            // 4. Очистка (на случай если в папке остались какие-то неучтенные файлы)
            await _fileStorage.DeleteDirectoryAsync(sourceFolder);
        }

        /// <summary>
        /// Заменяет часть ключа (пути) S3
        /// </summary>
        private string UpdateS3Path(string originalPath, string oldBase, string newBase)
        {
            if (string.IsNullOrEmpty(originalPath)) return originalPath;

            // Убираем начальные слеши для корректной замены
            string path = originalPath.TrimStart('/');
            string search = oldBase.TrimStart('/');
            string replace = newBase.TrimStart('/');

            return path.Replace(search, replace, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Получить мета-данные списка файлов
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<List<FileMetadata>> GetMetadataListAsync(List<string> keys)
        {
            var result = new List<FileMetadata>();
            if (keys == null) return result;
            foreach (var key in keys)
            {
                result.Add(await _fileStorage.GetFileMetadataAsync(key));
            }
            return result;
        }
    }
}
