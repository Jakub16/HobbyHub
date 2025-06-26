using FluentAssertions;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.AddEventMember;
using HobbyHub.Application.RequestHandlers.Events.CreateEvent;
using HobbyHub.Application.RequestHandlers.Events.DeleteEvent;
using HobbyHub.Application.RequestHandlers.Events.DeleteEventMember;
using HobbyHub.Application.RequestHandlers.Events.GetEvent;
using HobbyHub.Application.RequestHandlers.Events.GetEventMembers;
using HobbyHub.Application.RequestHandlers.Events.UpdateEvent;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Events;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HobbyHub.WebApi.UnitTests.Controllers;

public class EventsControllerTests
{
    private Mock<IHobbyHubRequestHandler> _requestHandler;
    private EventsController _sut;

    private static readonly Error NotFoundError = new Error("test", "test", StatusCodes.Status404NotFound);
    private static readonly Error BadRequestError = new Error("test", "test", StatusCodes.Status400BadRequest);

    [SetUp]
    public void SetUp()
    {
        _requestHandler = new Mock<IHobbyHubRequestHandler>();
        _sut = new EventsController(_requestHandler.Object);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateEventShouldReturnProperResult(bool isError)
    {
        var request = new CreateEventRequest
        {
            Title = "title",
            Description = "description"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateEventCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateEvent(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateEventCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateEventCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateEvent(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateEventCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task UpdateEventShouldReturnProperResult(bool isError)
    {
        var request = new UpdateEventRequest
        {
            EventId = 1,
            Title = "title",
            Description = "description"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<UpdateEventCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.UpdateEvent(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<UpdateEventCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<UpdateEventCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.UpdateEvent(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<UpdateEventCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task AddEventMemberShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<AddEventMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.AddEventMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<AddEventMemberCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<AddEventMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.AddEventMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<AddEventMemberCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteEventMemberShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteEventMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.DeleteEventMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteEventMemberCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteEventMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.DeleteEventMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteEventMemberCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteEventShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);
    
        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteEventCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));
    
            var result = await _sut.DeleteEvent(1);
    
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteEventCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteEventCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));
    
            var result = await _sut.DeleteEvent(1);
    
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteEventCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse>>();
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task GetEventShouldReturnProperResult(bool isError)
    {
        var response = new EventResponse
        {
            Title = "title"
        };
    
        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetEventQuery>()))
                .ReturnsAsync(Result<EventResponse>.Failure(NotFoundError));
    
            var result = await _sut.GetEvent(1);
    
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetEventQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetEventQuery>()))
                .ReturnsAsync(Result<EventResponse>.Success(response));
    
            var result = await _sut.GetEvent(1);
    
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetEventQuery>()));
            result.Should().BeOfType<ActionResult<EventResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetEventMembersShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<UserSummaryResponse>();
    
        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetEventMembersQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Failure(NotFoundError));
    
            var result = await _sut.GetEventMembers(1);
    
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetEventMembersQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetEventMembersQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Success(response));
    
            var result = await _sut.GetEventMembers(1);
    
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetEventMembersQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<UserSummaryResponse>>>();
        }
    }
}