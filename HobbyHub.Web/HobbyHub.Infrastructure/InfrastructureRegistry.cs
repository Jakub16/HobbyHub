using System.Diagnostics.CodeAnalysis;
using HobbyHub.Repository.Abstractions;
using HobbyHub.Repository.Repositories;
using HobbyHub.S3.Driver;
using HobbyHub.S3.Driver.Abstractions;
using HobbyHub.UserIdentity.Provider.Abstractions;
using HobbyHub.UserIdentity.Provider;
using StructureMap;

namespace HobbyHub.Infrastructure;

[ExcludeFromCodeCoverage]
public class InfrastructureRegistry : Registry
{
    public InfrastructureRegistry()
    {
        For<IFileUploader>().Use<FileUploader>();
        For<IPasswordHasher>().Use<PasswordHasher>();
        For<IJwtService>().Use<JwtService>();
        For<IHobbyHubRepository>().Use<HobbyHubRepository>();
    }
}