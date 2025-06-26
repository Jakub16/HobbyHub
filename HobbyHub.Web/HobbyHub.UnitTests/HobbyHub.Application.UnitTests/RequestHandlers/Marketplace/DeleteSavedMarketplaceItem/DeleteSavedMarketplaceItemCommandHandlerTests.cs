using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Marketplace.DeleteSavedMarketplaceItem;
using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Marketplace.DeleteSavedMarketplaceItem;

public class DeleteSavedMarketplaceItemCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private DeleteSavedMarketplaceItemCommandHandler _sut;
    
    private static readonly DeleteSavedMarketplaceItemCommand Command = new DeleteSavedMarketplaceItemCommand(
        new DeleteSavedMarketplaceItemRequest()
        {
            MarketplaceItemId = 1,
            UserId = 1
        });
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _sut = new DeleteSavedMarketplaceItemCommandHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenMarketplaceItemIsNotSaved()
    {
        _repository
            .Setup(repository =>
                repository.SavedMarketplaceItemsRepository.IsMarketplaceItemSaved(1, 1, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(MarketplaceErrors.SavedMarketplaceItemNotFoundCommand(1, 1));
    }
    
    [Test]
    public async Task ShouldHandleCreateSavedMarketplaceItemRequest()
    {
        _repository
            .Setup(repository =>
                repository.SavedMarketplaceItemsRepository.IsMarketplaceItemSaved(1, 1, CancellationToken.None))
            .ReturnsAsync(true);
        
        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.SavedMarketplaceItemsRepository.Delete(1, 1, CancellationToken.None), Times.Once);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}