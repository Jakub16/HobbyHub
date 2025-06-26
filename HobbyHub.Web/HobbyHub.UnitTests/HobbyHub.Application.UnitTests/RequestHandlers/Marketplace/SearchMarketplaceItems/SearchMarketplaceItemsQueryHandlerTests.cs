using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Marketplace;
using HobbyHub.Application.RequestHandlers.Marketplace.SearchMarketplaceItems;
using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.Repository.Abstractions.MarketplaceItems;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Marketplace.SearchMarketplaceItems;

public class SearchMarketplaceItemsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMarketplaceItemMapper> _marketplaceItemMapper;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _logger;
    private SearchMarketplaceItemsQueryHandler _sut;
    
    private static readonly User User = new User
    {
        UserId = 1,
        Email = "test@test.com",
        Name = "Name",
        Surname = "Surname"
    };
    
    private static readonly SearchMarketplaceItemsQuery Query = new SearchMarketplaceItemsQuery(
        new SearchMarketplaceItemsFilters()
        {
            UserId = 1,
            Keyword = "test",
            PageNumber = 1,
            PageSize = 20
        });
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _marketplaceItemMapper = new Mock<IMarketplaceItemMapper>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger>();
        _sut = new SearchMarketplaceItemsQueryHandler(_repository.Object, _mapper.Object, _marketplaceItemMapper.Object, _logger.Object);
    }
    
    [Test]
    public async Task ShouldHandleGetUserOrderedMarketplaceItemsQuery()
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
        
        _repository.Setup(repository => repository.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(true);
        
        _repository.Setup(repository => repository.MarketplaceItemsRepository
                .SearchMarketPlaceItems(Query.Filters, CancellationToken.None))
            .ReturnsAsync(new MarketplaceItemsWithPaging()
            {
                MarketplaceItems = marketplaceItems,
                TotalRecords = 2,
                TotalPages = 1
            });
        
        _repository.Setup(repository => repository.SavedMarketplaceItemsRepository
                .GetSavedMarketplaceItems(1, CancellationToken.None))
            .ReturnsAsync(marketplaceItems);

        _repository.Setup(repository => repository.UsersRepository
                .GetUsersByIds(It.IsAny<List<int>>(), CancellationToken.None))
            .ReturnsAsync([]);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userSummary);
        
        _marketplaceItemMapper.Setup(marketplaceItemMapper => marketplaceItemMapper
                .Map(marketplaceItems, It.IsAny<Dictionary<int, UserSummaryResponse>>()))
            .Returns(marketplaceItemsResponses);
        
        var result = await _sut.Handle(Query, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Data.Items.Should().BeEquivalentTo(marketplaceItemsResponses);
        result.Outcome.Data.Count.Should().Be(marketplaceItemsResponses.Count);
        result.Outcome.PageNumber.Should().Be(1);
        result.Outcome.PageSize.Should().Be(20);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}