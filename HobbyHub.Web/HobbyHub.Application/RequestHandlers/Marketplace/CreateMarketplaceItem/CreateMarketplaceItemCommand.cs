using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Marketplace;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Marketplace.CreateMarketplaceItem;

public class CreateMarketplaceItemCommand(CreateMarketplaceItemRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public string Title { get; } = request.Title;
    public string? Description { get; } = request.Description;
    public double Price { get; } = request.Price;
    public string Type { get; } = request.Type;
    public string Category { get; } = request.Category;
    public string? PhoneNumber { get; } = request.PhoneNumber;
    public string? Country { get; } = request.Country;
    public string? City { get; } = request.City;
    public string? PostalCode { get; } = request.PostalCode;
    public List<IFormFile>? Pictures { get; } = request.Pictures;
}