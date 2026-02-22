using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.Extensions.FileProviders;
using System.Threading.Tasks;

namespace LingvoGameOs.Helpers
{
    public class GameFileProcessor
    {
        readonly S3Service _s3Service;

        public GameFileProcessor(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        public async Task ProcessChangeCoverImageAsync(EditGameViewModel editGame, Game existingGame, Folders folder)
        {
            if (editGame.CoverImage != null)
            {
                string? newCoverImagePath = await _s3Service.UploadGameFileAsync(editGame.CoverImage, existingGame.Id, folder);
                string? oldCoverImagePath = editGame.CurrentCoverImagePath;

                existingGame.CoverImagePath = newCoverImagePath;

                if (newCoverImagePath != oldCoverImagePath)
                    await _s3Service.DeleteFileAsync(oldCoverImagePath!);
            }
            else
                existingGame.CoverImagePath = editGame.CurrentCoverImagePath!;
        }

        public async Task ProcessDeletedImagesAsync(EditGameViewModel editGameViewModel, Game game)
        {
            if (editGameViewModel.DeletedImages == null || !editGameViewModel.DeletedImages.Any())
                return;

            // Удаляем файлы из хранилища
            await _s3Service.DeleteFilesAsync(editGameViewModel.DeletedImages);

            // Оставляем у модели игры в БД только не удаленные картинки
            if (game.ImagesPaths != null)
            {
                var deletedImagesKeys = editGameViewModel.DeletedImages;
                game.ImagesPaths = game.ImagesPaths
                    .Where(imgPath => !deletedImagesKeys.Contains(imgPath))
                    .ToList();
            }
        }

        public async Task ProcessNewImagesAsync(EditGameViewModel editGameViewModel, Game game, Folders folder)
        {
            if (editGameViewModel.UploadedImages == null || !editGameViewModel.UploadedImages.Any(f => f.Length > 0))
                return;

            List<string> newImagesKeys = await _s3Service.UploadGameFilesAsync(
                editGameViewModel.UploadedImages.Where(f => f.Length > 0).ToArray(),
                editGameViewModel.Id,
                folder);

            if (game.ImagesPaths == null)
                game.ImagesPaths = new List<string>();
            game.ImagesPaths.AddRange(newImagesKeys);
        }

        public async Task ProcessDeleteGameFileAsync(EditGameViewModel editGame, Game existingGame)
        {
            if ((editGame.GamePlatform == "Desktop" && editGame.GameFilePath == null) || (editGame.GamePlatform != "Desktop" && editGame.GameFilePath != null))
            {
                if (editGame.CurrentGameFilePath != null)
                {
                    existingGame.GameFilePath = null;
                    await _s3Service.DeleteFileAsync(editGame.CurrentGameFilePath);
                }
            }
        }
    }
}
