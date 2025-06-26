using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Tutorials;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Tutorials;

public class TutorialsRepository(AppDbContext dbContext) : ITutorialsRepository
{

    public async Task<Tutorial?> GetTutorialById(int tutorialId, CancellationToken cancellationToken)
    {
        return await dbContext.Tutorials
            .Include(tutorial => tutorial.TutorialSteps)
            .ThenInclude(tutorialStep => tutorialStep.TutorialStepPictures)
            .Include(tutorial => tutorial.Users)
            .FirstOrDefaultAsync(tutorial => tutorial.TutorialId == tutorialId,
            cancellationToken);
    }

    public async Task<List<Tutorial>> GetAllTutorials(CancellationToken cancellationToken)
    {
        return await dbContext.Tutorials
            .AsNoTracking()
            .Include(tutorial => tutorial.TutorialSteps)
            .ThenInclude(tutorialStep => tutorialStep.TutorialStepPictures)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Tutorial>> GetUserTutorials(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.Tutorials
            .AsNoTracking()
            .Include(tutorial => tutorial.TutorialSteps)
            .ThenInclude(tutorialStep => tutorialStep.TutorialStepPictures)
            .Where(tutorial => tutorial.CreatedBy == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Tutorial>> GetUserAssignedTutorials(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.Tutorials
            .AsNoTracking()
            .Include(tutorial => tutorial.TutorialSteps)
            .ThenInclude(tutorialStep => tutorialStep.TutorialStepPictures)
            .Include(tutorial => tutorial.Users)
            .Where(tutorial => tutorial.Users.Any(user => user.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<TutorialsWithPaging> SearchTutorials(SearchTutorialsFilters filters, CancellationToken cancellationToken)
    {
        var userAssignedTutorials = await GetUserAssignedTutorials(filters.UserId, cancellationToken);
        var userAssignedTutorialsIds = userAssignedTutorials.Select(tutorial => tutorial.TutorialId).ToList();
        
        var query = dbContext.Tutorials
            .AsNoTracking()
            .AsQueryable()
            .Where(tutorial => !userAssignedTutorialsIds.Contains(tutorial.TutorialId));
        
        if (filters.OnlyForFree)
        {
            query = query.Where(tutorial => tutorial.Price == 0);
        }
        
        if (filters.CreatedBy != null)
        {
            query = query.Where(tutorial => tutorial.CreatedBy == filters.CreatedBy);
        }
        else
        {
            query = query.Where(tutorial => tutorial.CreatedBy != filters.UserId);
        }
        
        if (!string.IsNullOrEmpty(filters.Category))
        {
            query = query.Where(tutorial =>
                tutorial.Category.ToLower() == filters.Category.ToLower());
        }
        
        if (!string.IsNullOrEmpty(filters.Keyword))
        {
            query = query.Where(tutorial =>
                EF.Functions.Like(tutorial.Title.ToLower(), $"%{filters.Keyword.ToLower()}%")
                || (tutorial.Description != null && EF.Functions.Like(tutorial.Description.ToLower(),
                    $"%{filters.Keyword.ToLower()}%")));
        }

        if (filters.PriceRange != null)
        {
            if (filters.PriceRange.From != 0)
            {
                query = query.Where(tutorial => tutorial.Price >= filters.PriceRange.From);
            }
            
            if (filters.PriceRange.To != 0)
            {
                query = query.Where(tutorial => tutorial.Price <= filters.PriceRange.To);
            }
        }

        if (filters.PriceSortDirection != null)
        {
            query = filters.PriceSortDirection switch
            {
                "asc" => query.OrderBy(tutorial => tutorial.Price),
                "desc" => query.OrderByDescending(tutorial => tutorial.Price),
                _ => query.OrderByDescending(tutorial => tutorial.TutorialId)
            };
        }
        else
        {
            query = query.OrderByDescending(tutorial => tutorial.TutorialId);
        }
        
        var totalRecords = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize);
            
        var tutorials = await query
            .Include(tutorial => tutorial.Users)
            .Include(tutorial => tutorial.TutorialSteps)
            .ThenInclude(tutorialStep => tutorialStep.TutorialStepPictures)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);
        
        return new TutorialsWithPaging
        {
            Tutorials = tutorials,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }
}