using FluentAssertions;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.GetGroupEvents;
using HobbyHub.Application.RequestHandlers.Groups.AddGroupMember;
using HobbyHub.Application.RequestHandlers.Groups.CreateGroup;
using HobbyHub.Application.RequestHandlers.Groups.CreateGroupPost;
using HobbyHub.Application.RequestHandlers.Groups.CreateOrUpdateGroupMainPicture;
using HobbyHub.Application.RequestHandlers.Groups.DeleteGroupMember;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupPosts;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupSummary;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupUsers;
using HobbyHub.Application.RequestHandlers.Groups.GetPublicGroups;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Groups;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HobbyHub.WebApi.UnitTests.Controllers;

public class GroupsControllerTests
{
    private Mock<IHobbyHubRequestHandler> _requestHandler;
    private GroupsController _sut;

    private static readonly Error NotFoundError = new Error("test", "test", StatusCodes.Status404NotFound);
    private static readonly Error BadRequestError = new Error("test", "test", StatusCodes.Status400BadRequest);

    [SetUp]
    public void SetUp()
    {
        _requestHandler = new Mock<IHobbyHubRequestHandler>();
        _sut = new GroupsController(_requestHandler.Object);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateGroupShouldReturnProperResult(bool isError)
    {
        var request = new CreateGroupRequest()
        {
            UserId = 1,
            Name = "name"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateGroupCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateGroup(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateGroupCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateGroupCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateGroup(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateGroupCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task AddGroupMemberShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<AddGroupMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.AddGroupMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<AddGroupMemberCommand>()));
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<AddGroupMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.AddGroupMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<AddGroupMemberCommand>()));
            result.Should().BeOfType<NoContentResult>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteGroupMemberShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteGroupMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.DeleteMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteGroupMemberCommand>()));
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteGroupMemberCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.DeleteMember(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteGroupMemberCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetGroupMembersShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<UserSummaryResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupUsersQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Failure(NotFoundError));

            var result = await _sut.GetGroupUsers(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupUsersQuery>()));
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupUsersQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Success(response));

            var result = await _sut.GetGroupUsers(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupUsersQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<UserSummaryResponse>>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetGroupSummaryShouldReturnProperResult(bool isError)
    {
        var response = new GroupSummaryResponse()
        {
            Name = "name"
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupSummaryQuery>()))
                .ReturnsAsync(Result<GroupSummaryResponse>.Failure(NotFoundError));

            var result = await _sut.GetGroupSummary(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupSummaryQuery>()));
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupSummaryQuery>()))
                .ReturnsAsync(Result<GroupSummaryResponse>.Success(response));

            var result = await _sut.GetGroupSummary(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupSummaryQuery>()));
            result.Should().BeOfType<ActionResult<GroupSummaryResponse>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateGroupPostShouldReturnProperResult(bool isError)
    {
        var request = new CreateGroupPostRequest()
        {
            GroupId = 1,
            UserId = 1,
            Content = "content"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateGroupPostCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateGroupPost(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateGroupPostCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateGroupPostCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateGroupPost(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateGroupPostCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetGroupPostsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<GroupPostResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupPostsQuery>()))
                .ReturnsAsync(Result<ListResponse<GroupPostResponse>>.Failure(NotFoundError));

            var result = await _sut.GetGroupPosts(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupPostsQuery>()));
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupPostsQuery>()))
                .ReturnsAsync(Result<ListResponse<GroupPostResponse>>.Success(response));

            var result = await _sut.GetGroupPosts(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupPostsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<GroupPostResponse>>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetGroupEventsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<EventResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupEventsQuery>()))
                .ReturnsAsync(Result<ListResponse<EventResponse>>.Failure(NotFoundError));

            var result = await _sut.GetGroupEvents(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupEventsQuery>()));
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetGroupEventsQuery>()))
                .ReturnsAsync(Result<ListResponse<EventResponse>>.Success(response));

            var result = await _sut.GetGroupEvents(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetGroupEventsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<EventResponse>>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateOrUpdateGroupMainPictureShouldReturnProperResult(bool isError)
    {
        var request = new CreateOrUpdateGroupMainPictureRequest()
        {
            Picture = new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateOrUpdateGroupMainPictureCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.MainPicture(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateOrUpdateGroupMainPictureCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateOrUpdateGroupMainPictureCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.MainPicture(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateOrUpdateGroupMainPictureCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetPublicGroupsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<GroupSummaryResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetPublicGroupsQuery>()))
                .ReturnsAsync(Result<ListResponse<GroupSummaryResponse>>.Failure(NotFoundError));

            var result = await _sut.PublicGroups();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetPublicGroupsQuery>()));
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetPublicGroupsQuery>()))
                .ReturnsAsync(Result<ListResponse<GroupSummaryResponse>>.Success(response));

            var result = await _sut.PublicGroups();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetPublicGroupsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<GroupSummaryResponse>>>();
        }
    }
}