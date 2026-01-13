using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace LingvoGameOs.Helpers
{
    public class S3Service
    {
        readonly IAmazonS3 _s3Client;
        readonly string _bucketName;
        
        public S3Service(IConfiguration configuration)
        {
            string awsAccessKey;
            string awsKey;
            string awsServiceUrl;
            
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
            awsServiceUrl = configuration["AWS_TIMEWEB_SERVICE_URL"] ?? "";
            _bucketName = configuration["AWS_TIMEWEB_BUCKET_NAME"] ?? "";

            AmazonS3Config s3Config = new AmazonS3Config
            {
                ServiceURL = awsServiceUrl,
                ForcePathStyle = true,
                AuthenticationRegion = "ru-1"
            };
            _s3Client = new AmazonS3Client(awsAccessKey, awsKey, s3Config);
        }


        /// <summary>
        /// Получение ссылки на файл в S3 с истечением срока действия
        /// </summary>
        /// <param name="key">Уникальное имя файла</param>
        /// <param name="expirationMinutes">Время истечения ссылки</param>
        /// <returns></returns>
        public string GetFileUrl(string key, int expirationMinutes = 60)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
            return _s3Client.GetPreSignedURL(request);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string key)
        {
            using var newStream = new MemoryStream();
            await file.CopyToAsync(newStream);
            newStream.Position = 0; // КРИТИЧНО для корректной загрузки

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newStream,
                Key = key.TrimStart('/'),
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
            return key;
        }

        public async Task DeleteFileAsync(string key)
        {
            await _s3Client.DeleteObjectAsync(_bucketName, key.TrimStart('/'));
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
}
