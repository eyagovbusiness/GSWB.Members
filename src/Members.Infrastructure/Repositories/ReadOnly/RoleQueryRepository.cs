using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using Members.Infrastructure.DataAccess.DbContext;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB.Repository.CQRS;

namespace Members.Infrastructure.Repositories.ReadOnly
{
    internal class RoleQueryRepository(ReadOnlyMembersDbContext readOnlyContext, ILogger<RoleQueryRepository> aLogger) 
        : QueryRepositoryBase<RoleQueryRepository, ReadOnlyMembersDbContext, Role,ulong>(readOnlyContext, aLogger), IRoleQueryRepository 
    {
    }
}
