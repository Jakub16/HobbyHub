using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Marketplace.CreateMarketplaceItem;
using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Marketplace.CreateMarketplaceItem;

public class CreateMarketplaceItemCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFileUploader> _s3Uploader;
    private Mock<ILogger> _logger;
    private CreateMarketplaceItemCommandHandler _sut;
    
    private static readonly CreateMarketplaceItemCommand Command = new(new CreateMarketplaceItemRequest()
    {
        UserId = 1,
        City = "Test city",
        Country = "Test country",
        PostalCode = "12345",
        Title = "Test Title",
        Description = "Test description",
        PhoneNumber = "1234567890",
        Price = 100,
        Type = "Test type",
        Category = "Test category",
        Pictures = new List<IFormFile>()
        {
            new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
        }
    });

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _s3Uploader = new Mock<IFileUploader>();
        _logger = new Mock<ILogger>();
        _sut = new CreateMarketplaceItemCommandHandler(_repository.Object, _s3Uploader.Object, _logger.Object);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(Command.UserId));
    }

    [Test]
    public async Task ShouldHandleCreateMarketplaceItemRequest()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Email = "test@test.com",
                Name = "Name",
                Surname = "Surname"
            });
        
        _repository.Setup(r => r.MarketplaceItemsRepository.Add(It.IsAny<MarketplaceItem>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
        
        _repository.Setup(r => r.MarketplaceItemPicturesRepository.Add(It.IsAny<MarketplaceItemPicture>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        _repository.Setup(r => r.MarketplaceItemPicturesRepository
                .GetMarketplaceItemPictureById(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(new MarketplaceItemPicture());
        
        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.MarketplaceItemsRepository.Add(It.IsAny<MarketplaceItem>(), CancellationToken.None), Times.Once);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _s3Uploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
}