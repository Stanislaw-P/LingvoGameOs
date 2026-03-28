using LingvoGameOs.Areas.Admin.Models;

namespace LingvoGameOs.Helpers
{
    public interface IFileStorage
    {
        // Загрузка
        Task<string> UploadGameFileAsync(IFormFile file, int gameId, Folders folder);
        Task<string> UploadAvatarFileAsync(IFormFile file,  Folders folder);
        Task<List<string>> UploadGameFilesAsync(IFormFile[] files, int gameId, Folders folder);

        // Ссылки
        string? GetPublicUrl(string key);
        string GetDownloadUrl(string key, string displayTitle, string extension);

        // Удаление и манипуляции
        Task DeleteFileAsync(string key);
        Task DeleteDirectoryAsync(string prefix);
        Task MoveFileAsync(string sourceKey, string destinationKey);

        // Метаданные
        Task<FileMetadata> GetFileMetadataAsync(string key);
    }
}