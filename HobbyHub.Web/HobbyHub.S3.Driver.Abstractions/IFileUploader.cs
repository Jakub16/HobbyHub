using Microsoft.AspNetCore.Http;

namespace HobbyHub.S3.Driver.Abstractions;

public interface IFileUploader
{
    Task<string> Send(string destination, string name, IFormFile file);
}