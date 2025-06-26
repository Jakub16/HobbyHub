using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Marketplace;
using HobbyHub.Application.RequestHandlers.Marketplace.GetMarketplaceItems;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Marketplace.GetMarketplaceItems;

public class GetMarketplaceItemsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMarketplaceItemMapper> _marketplaceItemMapper;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _logger;
    private GetMarketplaceItemsQueryHandler _sut;
    
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
        _marketplaceItemMapper = new Mock<IMarketplaceItemMapper>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger>();
        _sut = new GetMarketplaceItemsQueryHandler(_repository.Object, _mapper.Object, _marketplaceItemMapper.Object, _logger.Object);
    }
    
    [Test]
    public async Task ShouldHandleGetMarketplaceItemsQuery()
    {
        var marketplaceItems = new List<MarketplaceItem>
        {
            new MarketplaceItem()
            {
                MarketplaceItemId = 1,
                Title = "title1",
                Description = "description1",
                Category = "category1",
                Type = "type1",
                User = User
            },
            new MarketplaceItem()
            {
                MarketplaceItemId = 2,
                Title = "title2",
                Description = "description2",
                Category = "category2",
                Type = "type2",
                User = User
            }
        };

        var marketplaceItemsResponses = new List<MarketplaceItemResponse>
        {
            new MarketplaceItemResponse()
            {
                MarketplaceItemId = 1,
                Title = "title 1",
                Description = "description1",
                Category = "category1",
                Type = "type1"
            },
            new MarketplaceItemResponse()
            {
                MarketplaceItemId = 2,
                Title = "title2",
                Description = "description2",
                Category = "category2",
                Type = "type2"
            },
        };
        
        var userSummary = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name",
            Surname = "surname"
        };
        
        _repository.Setup(repository => repository.MarketplaceItemsRepository
                .GetMarketplaceItems(CancellationToken.None))
            .ReturnsAsync(marketplaceItems);

        _repository.Setup(repository => repository.UsersRepository
                .GetUsersByIds(It.IsAny<List<int>>(), CancellationToken.None))
            .ReturnsAsync([]);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userSummary);
        
        _marketplaceItemMapper.Setup(marketplaceItemMapper => marketplaceItemMapper
                .Map(marketplaceItems, It.IsAny<Dictionary<int, UserSummaryResponse>>()))
            .Returns(marketplaceItemsResponses);
        
        var result = await _sut.Handle(new GetMarketplaceItemsQuery(), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(marketplaceItemsResponses);
        result.Outcome.Count.Should().Be(marketplaceItemsResponses.Count);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}