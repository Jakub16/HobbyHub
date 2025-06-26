using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Database.Entities;

namespace HobbyHub.Application.UnitTests;

public static class Stub
{
    public static readonly List<Hobby> PredefinedHobbies =
    [
        new Hobby()
        {
            HobbyId = 1,
            Name = "Predefined Hobby 1",
            IconType = "icon 1"
        },
        new Hobby()
        {
            HobbyId = 2,
            Name = "Predefined Hobby 2",
            IconType = "icon 2"
        },
        new Hobby()
        {
            HobbyId = 3,
            Name = "Predefined Hobby 3",
            IconType = "icon 3"
        }
    ];
    
    public static readonly List<UserHobby> UserHobbies =
    [
        new UserHobby()
        {
            UserHobbyId = 1,
            Name = "Predefined Hobby 1",
            IconType = "icon 1"
        },
        new UserHobby()
        {
            UserHobbyId = 2,
            Name = "Predefined Hobby 2",
            IconType = "icon 2"
        },
        new UserHobby()
        {
            UserHobbyId = 3,
            Name = "Predefined Hobby 3",
            IconType = "icon 3"
        }
    ];
    
    public static readonly List<HobbyResponse> PredefinedHobbiesResponse =
    [
        new HobbyResponse()
        {
            HobbyId = 1,
            Name = "Predefined Hobby 1",
            IconType = "icon 1",
            IsPredefined = true
        },
        new HobbyResponse()
        {
            HobbyId = 2,
            Name = "Predefined Hobby 2",
            IconType = "icon 2",
            IsPredefined = true
        },
        new HobbyResponse()
        {
            HobbyId = 3,
            Name = "Predefined Hobby 3",
            IconType = "icon 3",
            IsPredefined = true
        }
    ];
    
    public static readonly List<HobbyResponse> UserHobbiesResponse =
    [
        new HobbyResponse()
        {
            HobbyId = 1,
            Name = "Predefined Hobby 1",
            IconType = "icon 1",
            IsPredefined = false
        },
        new HobbyResponse()
        {
            HobbyId = 2,
            Name = "Predefined Hobby 2",
            IconType = "icon 2",
            IsPredefined = false
        },
        new HobbyResponse()
        {
            HobbyId = 3,
            Name = "Predefined Hobby 3",
            IconType = "icon 3",
            IsPredefined = false
        }
    ];

    public static readonly Hobby PredefinedHobby = new Hobby()
        {
            HobbyId = 1,
            Name = "Predefined Hobby 1",
            IconType = "icon 1"
        };
}