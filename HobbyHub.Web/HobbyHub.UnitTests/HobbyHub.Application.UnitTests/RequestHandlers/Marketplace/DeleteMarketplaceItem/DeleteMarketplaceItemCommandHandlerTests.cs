using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Marketplace.DeleteMarketplaceItem;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Marketplace.DeleteMarketplaceItem;

public class DeleteMarketplaceItemCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private DeleteMarketplaceItemCommandHandler _sut;
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _sut = new DeleteMarketplaceItemCommandHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenMarketplaceItemDoesNotExist()
    {
        _repository
            .Setup(repository =>
                repository.MarketplaceItemsRepository.GetMarketplaceItemById(1, CancellationToken.None))
            .ReturnsAsync((MarketplaceItem)null!);

        var result = await _sut.Handle(new DeleteMarketplaceItemCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(MarketplaceErrors.MarketplaceItemNotFoundCommand(1));
    }
    
    [Test]
    public async Task ShouldHandleDeleteMarketplaceItemCommand()
    {
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
        
        var result = await _sut.Handle(new DeleteMarketplaceItemCommand(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.MarketplaceItemsRepository.Delete(It.IsAny<MarketplaceItem>(), CancellationToken.None), Times.Once);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}