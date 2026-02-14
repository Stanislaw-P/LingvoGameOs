using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Helpers
{
    public class S3Service
    {
        readonly IAmazonS3 _s3Client;
        readonly string _bucketName;
        readonly string _awsServiceUrl;

        public S3Service(IConfiguration configuration)
        {
            string awsAccessKey;
            string awsKey;

            //if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            //{
            //    awsAccessKey = configuration["AWS_YANDEX_KEY_ID"] ?? "";
            //    awsKey = configuration["AWS_YANDEX_KEY"] ?? "";
            //    awsServiceUrl = configuration["AWS_YANDEX_SERVICE_URL"] ?? "";
            //    _bucketName = configuration["AWS_YANDEX_BUCKET_NAME"] ?? "";
            //}
            //else
            //{
            //    awsAccessKey = configuration["AWS_TIMEWEB_KEY_ID"] ?? "";
            //    awsKey = configuration["AWS_TIMEWEB_KEY"] ?? "";
            //    awsServiceUrl = configuration["AWS_TIMEWEB_SERVICE_URL"] ?? "";
            //    _bucketName = configuration["AWS_TIMEWEB_BUCKET_NAME"] ?? "";
            //}

            awsAccessKey = configuration["AWS_TIMEWEB_KEY_ID"] ?? "";
            awsKey = configuration["AWS_TIMEWEB_KEY"] ?? "";
            _awsServiceUrl = configuration["AWS_TIMEWEB_SERVICE_URL"] ?? "";
            _bucketName = configuration["AWS_TIMEWEB_BUCKET_NAME"] ?? "";

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
        /// Загрузить аватар пользователя в S3
        /// </summary>
        /// <param name="file">Файл аватарки</param>
        /// <param name="userId">Id пользователя</param>
        /// <param name="folder">Имя папки в которую нужно сохранить файл (Avatars)</param>
        /// <returns>Уникальное имя файла в хранилище</returns>
        public async Task<string> UploadAvatarFileAsync(IFormFile file, string userId, Folders folder)
        {
            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{userId}{extension}";
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

            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }

        public async Task DeleteDirectoryAsync(string prefix)
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = prefix.TrimStart('/')
            };

            var listResponse = await _s3Client.ListObjectsV2Async(listRequest);

            if (listResponse.S3Objects.Any())
            {
                var deleteRequest = new DeleteObjectsRequest
                {
                    BucketName = _bucketName,
                    Objects = listResponse.S3Objects.Select(o => new KeyVersion { Key = o.Key }).ToList()
                };
                await _s3Client.DeleteObjectsAsync(deleteRequest);
            }
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
    }

    public record S3UploadResult(string Key, string PublicUrl);
}
