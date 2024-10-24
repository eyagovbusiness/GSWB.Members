using Microsoft.EntityFrameworkCore;
using TGF.CA.Infrastructure.DB.DbContext;
using Members.Domain.Entities;

namespace Members.Infrastructure.DataAccess.DbContext;

public class ReadOnlyMembersDbContext(DbContextOptions<MembersDbContext> options) : EntitiesDbContext<MembersDbContext>(options), IReadOnlyDbContext
{

    private DbSet<Role> Roles { get; set; }

    // Explicitly implement the IReadOnlyDbContext methods
    public IQueryable<TEntity> Query<TEntity>() where TEntity : class
    {
        return Set<TEntity>().AsNoTracking(); // Ensures that EF does not track changes
    }

    public async override ValueTask<TEntity?> FindAsync<TEntity>(params object?[]? keyValues) where TEntity : class
    {
        return await Set<TEntity>().FindAsync(keyValues);
    }
}
