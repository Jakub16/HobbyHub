using HobbyHub.Repository.Abstractions.Activities;
using HobbyHub.Repository.Abstractions.ActivityPictures;
using HobbyHub.Repository.Abstractions.Events;
using HobbyHub.Repository.Abstractions.FavoriteHobbies;
using HobbyHub.Repository.Abstractions.GroupPostPictures;
using HobbyHub.Repository.Abstractions.GroupPosts;
using HobbyHub.Repository.Abstractions.Groups;
using HobbyHub.Repository.Abstractions.Hobbies;
using HobbyHub.Repository.Abstractions.MarketplaceItemPictures;
using HobbyHub.Repository.Abstractions.MarketplaceItems;
using HobbyHub.Repository.Abstractions.Notes;
using HobbyHub.Repository.Abstractions.Posts;
using HobbyHub.Repository.Abstractions.SavedMarketplaceItems;
using HobbyHub.Repository.Abstractions.Tutorials;
using HobbyHub.Repository.Abstractions.TutorialStepPictures;
using HobbyHub.Repository.Abstractions.UserHobbies;
using HobbyHub.Repository.Abstractions.UserIdentity;
using HobbyHub.Repository.Abstractions.Users;

namespace HobbyHub.Repository.Abstractions;

public interface IHobbyHubRepository
{
    IHobbiesRepository HobbiesRepository { get; }
    IUsersRepository UsersRepository { get; }
    IUserHobbiesRepository UserHobbiesRepository { get; }
    IActivitiesRepository ActivitiesRepository { get; }
    IActivityPicturesRepository ActivityPicturesRepository { get; }
    INotesRepository NotesRepository { get; }
    IPostsRepository PostsRepository { get; }
    IUserIdentityRepository UserIdentityRepository { get; }
    IFavoriteHobbiesRepository FavoriteHobbiesRepository { get; }
    IGroupsRepository GroupsRepository { get; }
    IGroupPostsRepository GroupPostsRepository { get; }
    IGroupPostPicturesRepository GroupPostPicturesRepository { get; }
    IEventsRepository EventsRepository { get; }
    IMarketplaceItemsRepository MarketplaceItemsRepository { get; }
    IMarketplaceItemPicturesRepository MarketplaceItemPicturesRepository { get; }
    ITutorialsRepository TutorialsRepository { get; }
    ITutorialStepPicturesRepository TutorialStepPicturesRepository { get; }
    ISavedMarketplaceItemsRepository SavedMarketplaceItemsRepository { get; }
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add<T>(T item, CancellationToken cancellationToken);
}