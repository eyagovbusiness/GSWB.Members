using Common.Domain.ValueObjects;
using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using Members.Infrastructure.DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB;
using TGF.CA.Infrastructure.DB.Repository.CQRS;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Infrastructure.Repositories.ReadOnly
{
    internal class RoleQueryRepository(ReadOnlyMembersDbContext readOnlyContext, ILogger<RoleQueryRepository> aLogger)
        : QueryRepository<RoleQueryRepository, ReadOnlyMembersDbContext, Role>(readOnlyContext, aLogger), IRoleQueryRepository
    {

        public async Task<IHttpResult<Role>> GetByIdAsync(RoleKey roleKey, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async cancellationToken => {
            var entity = await Queryable
            .FirstOrDefaultAsync(role => role.GuildId == roleKey.GuildId && role.RoleId == roleKey.RoleId, cancellationToken);
            return entity != null 
            ? Result.SuccessHttp(entity) 
            : Result.Failure<Role>(DBErrors.Repository.Entity.NotFound);
        }, cancellationToken);


        public async Task<IHttpResult<IEnumerable<Role>>> GetByIdListAsync(IEnumerable<RoleKey> roleKeys, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async cancellationToken => {

            // Step 1: Filter based on RoleId and GuildId in the database to reduce the result set
            var preliminaryResults = await Queryable
                .Where(role => roleKeys.Select(rk => rk.RoleId).Contains(role.RoleId) &&
                               roleKeys.Select(rk => rk.GuildId).Contains(role.GuildId))
                .ToListAsync(cancellationToken);

            // Step 2: Filter the preliminary results in-memory to ensure exact composite key matching
            var entities = preliminaryResults
                .Where(role => roleKeys.Contains(new RoleKey(role.GuildId, role.RoleId)))
                .ToList();
            return entities.Count != 0 ? Result.SuccessHttp(entities as IEnumerable<Role>) : Result.Failure<IEnumerable<Role>>(DBErrors.Repository.Entity.NotFound);
        }, cancellationToken);

    }

}
