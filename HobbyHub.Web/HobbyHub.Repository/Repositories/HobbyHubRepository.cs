using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions;
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
using HobbyHub.Repository.Repositories.Activities;
using HobbyHub.Repository.Repositories.ActivityPictures;
using HobbyHub.Repository.Repositories.Events;
using HobbyHub.Repository.Repositories.FavoriteHobby;
using HobbyHub.Repository.Repositories.GroupPostPictures;
using HobbyHub.Repository.Repositories.GroupPosts;
using HobbyHub.Repository.Repositories.Groups;
using HobbyHub.Repository.Repositories.Hobbies;
using HobbyHub.Repository.Repositories.MarketplaceItemPictures;
using HobbyHub.Repository.Repositories.MarketplaceItems;
using HobbyHub.Repository.Repositories.Notes;
using HobbyHub.Repository.Repositories.Posts;
using HobbyHub.Repository.Repositories.SavedMarketplaceItems;
using HobbyHub.Repository.Repositories.Tutorials;
using HobbyHub.Repository.Repositories.TutorialStepPictures;
using HobbyHub.Repository.Repositories.UserHobbies;
using HobbyHub.Repository.Repositories.UserIdentity;
using HobbyHub.Repository.Repositories.Users;


namespace HobbyHub.Repository.Repositories;

public class HobbyHubRepository(AppDbContext appDbContext) : IHobbyHubRepository
{
    public IHobbiesRepository HobbiesRepository { get; } = new HobbiesRepository(appDbContext);
    public IUsersRepository UsersRepository { get; } = new UsersRepository(appDbContext);
    public IUserHobbiesRepository UserHobbiesRepository { get; } = new UserHobbiesRepository(appDbContext);
    public IActivitiesRepository ActivitiesRepository { get; } = new ActivitiesRepository(appDbContext);
    public IActivityPicturesRepository ActivityPicturesRepository { get; } =
        new ActivityPicturesRepository(appDbContext);
    public INotesRepository NotesRepository { get; } = new NotesRepository(appDbContext);
    public IPostsRepository PostsRepository { get; } = new PostsRepository(appDbContext);
    public IUserIdentityRepository UserIdentityRepository { get; } = new UserIdentityRepository(appDbContext);
    public IFavoriteHobbiesRepository FavoriteHobbiesRepository { get; } = new FavoriteHobbiesRepository(appDbContext);
    public IGroupsRepository GroupsRepository { get; } = new GroupsRepository(appDbContext);
    public IGroupPostsRepository GroupPostsRepository { get; } = new GroupPostsRepository(appDbContext);
    public IGroupPostPicturesRepository GroupPostPicturesRepository { get; } =
        new GroupPostPicturesRepository(appDbContext);
    public IEventsRepository EventsRepository { get; } = new EventsRepository(appDbContext);
    public IMarketplaceItemsRepository MarketplaceItemsRepository { get; } =
        new MarketplaceItemsRepository(appDbContext);
    public IMarketplaceItemPicturesRepository MarketplaceItemPicturesRepository { get; } =
        new MarketplaceItemPicturesRepository(appDbContext);
    public ITutorialsRepository TutorialsRepository { get; } = new TutorialsRepository(appDbContext);
    public ITutorialStepPicturesRepository TutorialStepPicturesRepository { get; } =
        new TutorialStepPicturesRepository(appDbContext);
    public ISavedMarketplaceItemsRepository SavedMarketplaceItemsRepository { get; } =
        new SavedMarketplaceItemsRepository(appDbContext);

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add<T>(T item, CancellationToken cancellationToken)
    {
        await appDbContext.AddAsync(item, cancellationToken);
    }
}