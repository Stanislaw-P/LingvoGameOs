using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using LingvoGameOs.Areas.Admin.Models;
using Sprache;

namespace LingvoGameOs.Helpers
{
    public class S3Service : IFileStorage
    {
        readonly IAmazonS3 _s3Client;
        readonly string? _bucketName = null;
        readonly string? _awsServiceUrl = null;

        public S3Service(IConfiguration configuration)
        {
            string? awsAccessKey = null;
            string? awsKey = null;

            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Test")
            {
                awsAccessKey = configuration["AWS_YANDEX_KEY_ID"] ?? "";
                awsKey = configuration["AWS_YANDEX_KEY"] ?? "";
                _awsServiceUrl = configuration["AWS_YANDEX_SERVICE_URL"] ?? "";
                _bucketName = configuration["AWS_YANDEX_BUCKET_NAME"] ?? "";
            }
            else if (configuration["ASPNETCORE_ENVIRONMENT"] == "Production")
            {
                awsAccessKey = configuration["AWS_TIMEWEB_KEY_ID"] ?? "";
                awsKey = configuration["AWS_TIMEWEB_KEY"] ?? "";
                _awsServiceUrl = configuration["AWS_TIMEWEB_SERVICE_URL"] ?? "";
                _bucketName = configuration["AWS_TIMEWEB_BUCKET_NAME"] ?? "";
            }

            AmazonS3Config s3Config = new AmazonS3Config
            {
                ServiceURL = _awsServiceUrl,
                ForcePathStyle = true,
                AuthenticationRegion = "ru-1"
            };

            _s3Client = new AmazonS3Client(awsAccessKey, awsKey, s3Config);
        }

        /// <summary>
        /// Возвращает публичную ссылку на объект или null
        /// </summary>
        /// <param name="key">Уникальное имя объекта в хранилище S3</param>
        /// <returns></returns>
        public string? GetPublicUrl(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (key.StartsWith('/'))
                key = key.TrimStart('/');

            string baseUrl = $"{_awsServiceUrl}/{_bucketName}";
            return $"{baseUrl.TrimEnd('/')}/{key}";
        }

        /// <summary>
        /// Получение ссылки на файл в S3 с истечением срока действия
        /// </summary>
        /// <param name="key">Уникальное имя файла в хранилище S3</param>
        /// <param name="expirationMinutes">Время жизни ссылки</param>
        /// <returns></returns>
        public string GetPreSignedFileUrl(string key, int expirationMinutes = 60)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
            return _s3Client.GetPreSignedURL(request);
        }

        public async Task<string> UploadGameFileAsync(IFormFile file, int gameId, Folders folder)
        {
            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var key = $"{folder}/{gameId}/{uniqueName}";

            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
            };

            await _s3Client.PutObjectAsync(request);

            return key;
        }

        /// <summary>
        /// Загрузить несколько файлов игры
        /// </summary>
        /// <param name="files">Файлы для загрузки</param>
        /// <param name="gameId">Id игры</param>
        /// <param name="folder">Папка, в которую нужно загрузить файл</param>
        /// <returns></returns>
        public async Task<List<string>> UploadGameFilesAsync(IFormFile[] files, int gameId, Folders folder)
        {
            // 1. Создаем коллекцию задач (пока без await внутри Select)
            var uploadTasks = files
                            .Select(img => UploadGameFileAsync(img, gameId, folder));

            // 2. Дожидаемся завершения всех задач и получаем массив строк
            string[] uploadedPathsArray = await Task.WhenAll(uploadTasks);

            return uploadedPathsArray.ToList();
        }

        /// <summary>
        /// Загрузить аватар пользователя в S3
        /// </summary>
        /// <param name="file">Файл аватарки</param>
        /// <param name="folder">Папка, в которую нужно загрузить файл (Avatars)</param>
        /// <returns>Уникальное имя файла в хранилище</returns>
        public async Task<string> UploadAvatarFileAsync(IFormFile file, Folders folder)
        {
            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var key = $"{folder}/{uniqueName}";

            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
            };

            await _s3Client.PutObjectAsync(request);

            return key;
        }

        public async Task DeleteFileAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            key = key.TrimStart('/');

            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }

        public async Task DeleteFilesAsync(List<string> keys)
        {
            if (!keys.Any() || keys == null) return;

            var request = new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = keys
                    .Where(key => !string.IsNullOrEmpty(key))
                    .Select(key => new KeyVersion { Key = key.TrimStart('/') })
                    .ToList()
            };

            try
            {
                await _s3Client.DeleteObjectsAsync(request);
            }
            catch (DeleteObjectsException ex)
            {
                throw new Exception($"Ошибка при массовом удалении из S3: {ex.Message}");
            }
        }

        /// <summary>
        /// Удаляет все объекты с указанным префиксом (имитация удаления папки)
        /// </summary>
        /// <param name="prefix">Путь к папке, например "PendingGames/10"</param>
        public async Task DeleteDirectoryAsync(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return;

            // Нормализуем префикс, чтобы не удалить лишнего (например, Games/1 и Games/10)
            string normalizedPrefix = prefix.TrimStart('/').TrimEnd('/') + "/";
            string? continuationToken = null;

            do
            {
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = normalizedPrefix,
                    ContinuationToken = continuationToken
                };

                var listResponse = await _s3Client.ListObjectsV2Async(listRequest);

                if (listResponse.S3Objects != null && listResponse.S3Objects.Any())
                {
                    var deleteRequest = new DeleteObjectsRequest
                    {
                        BucketName = _bucketName,
                        Objects = listResponse.S3Objects
                            .Select(o => new KeyVersion { Key = o.Key })
                            .ToList()
                    };

                    await _s3Client.DeleteObjectsAsync(deleteRequest);
                }

                continuationToken = listResponse.NextContinuationToken;

            } while (continuationToken != null);
        }

        // Копирование одного файла
        public async Task CopyFileAsync(string sourceKey, string destinationKey)
        {
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                SourceKey = sourceKey.TrimStart('/'),
                DestinationBucket = _bucketName,
                DestinationKey = destinationKey.TrimStart('/')
            };

            await _s3Client.CopyObjectAsync(copyRequest);
        }

        // Перемещение одного файла (Копирование + Удаление)
        public async Task MoveFileAsync(string sourceKey, string destinationKey)
        {
            await CopyFileAsync(sourceKey, destinationKey);
            await DeleteFileAsync(sourceKey);
        }

        public async Task<List<FileMetadata>> GetFilesMetadataAsync(List<string> filesKeys)
        {
            var result = new List<FileMetadata>();

            foreach (var key in filesKeys)
            {
                try
                {
                    var metadata = await _s3Client.GetObjectMetadataAsync(_bucketName, key);

                    result.Add(new FileMetadata
                    {
                        Key = key,
                        FileUrl = GetPublicUrl(key),
                        Size = metadata.ContentLength,
                    });
                }
                catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Если файла нет в S3, просто пропускаем его или логируем
                    continue;
                }
            }

            return result;
        }

        public async Task<FileMetadata> GetFileMetadataAsync(string key)
        {
            try
            {
                var metadata = await _s3Client.GetObjectMetadataAsync(_bucketName, key);

                return new FileMetadata
                {
                    Key = key,
                    FileUrl = GetPublicUrl(key),
                    Size = metadata.ContentLength
                };
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Если файла нет в S3
                return new FileMetadata();
            }
        }

        public string GetDownloadUrl(string key, string displayTitle, string extension)
        {
            // Очищаем заголовок от лишних пробелов и кавычек
            string safeFileName = $"{displayTitle.Trim()}{extension}";

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(60),
                ResponseHeaderOverrides = new ResponseHeaderOverrides
                {
                    ContentDisposition = $"attachment; filename=\"{safeFileName}\""
                }
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }

    public record S3UploadResult(string Key, string PublicUrl);
}
