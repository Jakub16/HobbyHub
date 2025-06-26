using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace HobbyHub.S3.Driver;

public class FileUploader(ILogger log) : IFileUploader
{
    private const string BucketName = "hobbyhub";

    private static readonly BasicAWSCredentials Credentials = new ("AKIA2AUOO2HSA74YQG2H", "dSQNFW5Ucwh94DRJDj1iGNh5ForzKpmItH8XETtG");
    private readonly AmazonS3Client _s3Client = new (Credentials, Amazon.RegionEndpoint.EUNorth1);

    public async Task<string> Send(string destination, string name, IFormFile file)
    {
        var currentTime = DateTime.UtcNow;
        var unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        
        var keyName = $"{destination}/{name}_{unixTime}";
        var totalSize = file.Length;
        var fileBytes = new byte[file.Length];

        await using (var fileStream = file.OpenReadStream())
        {
            var offset = 0;

            while (offset < file.Length)
            {
                var chunkSize = totalSize - offset < 8192 ? (int) totalSize - offset : 8192;

                offset += await fileStream.ReadAsync(fileBytes.AsMemory(offset, chunkSize));
            }
        }
        
        await using var memoryStream = new MemoryStream(fileBytes);
        
        try
        {
            var request = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = keyName,
                InputStream = memoryStream,
                ContentType = "image/jpeg",
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };
            
            await _s3Client.PutObjectAsync(request);
            
            var url = $"https://hobbyhub.s3.eu-north-1.amazonaws.com/{keyName}";
            
            log.Information($"Successfully uploaded object with url {url}");

            return url;
        }
        catch (AmazonS3Exception ex)
        {
            log.Error($"Error: '{ex.Message}' when writing an object");
            throw;
        }
        catch (Exception ex)
        {
            log.Error($"Error: '{ex.Message}' when writing an object");
            throw;
        }
    }

}