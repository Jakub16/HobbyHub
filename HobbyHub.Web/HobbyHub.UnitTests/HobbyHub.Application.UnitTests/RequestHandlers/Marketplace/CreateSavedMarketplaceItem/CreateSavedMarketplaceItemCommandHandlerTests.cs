using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Marketplace.CreateSavedMarketplaceItem;
using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Marketplace.CreateSavedMarketplaceItem;

public class CreateSavedMarketplaceItemCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private CreateSavedMarketplaceItemCommandHandler _sut;

    private static readonly CreateSavedMarketplaceItemCommand Command = new CreateSavedMarketplaceItemCommand(
        new CreateSavedMarketplaceItemRequest()
        {
            MarketplaceItemId = 1,
            UserId = 1
        });

    private static readonly User User = new User
    {
        UserId = 1,
        Email = "test@test.com",
        Name = "Name",
        Surname = "Surname"
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _sut = new CreateSavedMarketplaceItemCommandHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenMarketplaceItemDoesNotExist()
    {
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);

        _repository
            .Setup(repository =>
                repository.MarketplaceItemsRepository.GetMarketplaceItemById(1, CancellationToken.None))
            .ReturnsAsync((MarketplaceItem)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(MarketplaceErrors.MarketplaceItemNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenMarketplaceItemIsAlreadySaved()
    {
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);

        _repository
            .Setup(repository =>
                repository.MarketplaceItemsRepository.GetMarketplaceItemById(1, CancellationToken.None))
            .ReturnsAsync(new MarketplaceItem()
            {
                MarketplaceItemId = 1,
                Title = "title",
                Price = 1.1,
                Type = "type",
                Category = "category"
            });
        
        _repository
            .Setup(repository =>
                repository.SavedMarketplaceItemsRepository.IsMarketplaceItemSaved(1, 1, CancellationToken.None))
            .ReturnsAsync(true);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(MarketplaceErrors.MarketplaceItemAlreadySavedCommand(1, 1));
    }
    
    [Test]
    public async Task ShouldHandleCreateSavedMarketplaceItemRequest()
    {
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository
            .Setup(repository =>
                repository.MarketplaceItemsRepository.GetMarketplaceItemById(1, CancellationToken.None))
            .ReturnsAsync(new MarketplaceItem()
            {
                MarketplaceItemId = 1,
                Title = "title",
                Price = 1.1,
                Type = "type",
                Category = "category"
            });

        _repository
            .Setup(repository =>
                repository.SavedMarketplaceItemsRepository.IsMarketplaceItemSaved(1, 1, CancellationToken.None))
            .ReturnsAsync(false);
        
        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.Add(It.IsAny<SavedMarketplaceItem>(), CancellationToken.None), Times.Once);
        _repository.Verify(r => r.SaveChanges(CancellationToken.None), Times.Once);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Exactly(2));
    }
}