using Microsoft.EntityFrameworkCore;
using TGF.CA.Infrastructure.DB.DbContext;
using Members.Domain.Entities;

namespace Members.Infrastructure.DataAccess.DbContext;

public class ReadOnlyMembersDbContext(DbContextOptions<ReadOnlyMembersDbContext> options) 
    : ReadOnlyEntitiesDbContext<ReadOnlyMembersDbContext>(options)
{
    private DbSet<Role> Roles { get; set; }

}
