using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;

namespace HobbyHub.Application.Infrastructure;

[ExcludeFromCodeCoverage]
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Hobby, HobbyResponse>();
        CreateMap<UserHobby, HobbyResponse>()
            .ForMember(dest => dest.HobbyId, opt => opt.MapFrom(src => src.UserHobbyId));
        CreateMap<Group, GroupSummaryResponse>();
        CreateMap<User, UserSummaryResponse>();
    }
}