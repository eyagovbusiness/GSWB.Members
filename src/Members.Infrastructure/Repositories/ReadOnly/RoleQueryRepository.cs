using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using Members.Domain.ValueObjects.Role;
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

        public async Task<IHttpResult<Role>> GetByIdAsync(ulong roleId, ulong guildId, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async cancellationToken => {
            var entity = await Queryable
            .FirstOrDefaultAsync(role => role.RoleId == roleId && role.GuildId == guildId, cancellationToken);
            return entity != null 
            ? Result.SuccessHttp(entity) 
            : Result.Failure<Role>(DBErrors.Repository.Entity.NotFound);
        }, cancellationToken);


        public async Task<IHttpResult<IEnumerable<Role>>> GetByIdListAsync(IEnumerable<(ulong RoleId, ulong GuildId)> roleIds, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async cancellationToken => {
            // Convert the roleIds to a list of RoleKey objects
            var roleKeys = roleIds.Select(id => new RoleKey(id.RoleId, id.GuildId)).ToList();

            // Step 1: Filter based on RoleId and GuildId in the database to reduce the result set
            var preliminaryResults = await Queryable
                .Where(role => roleKeys.Select(rk => rk.RoleId).Contains(role.RoleId) &&
                               roleKeys.Select(rk => rk.GuildId).Contains(role.GuildId))
                .ToListAsync(cancellationToken);

            // Step 2: Filter the preliminary results in-memory to ensure exact composite key matching
            var entities = preliminaryResults
                .Where(role => roleKeys.Contains(new RoleKey(role.RoleId, role.GuildId)))
                .ToList();
            return entities.Count != 0 ? Result.SuccessHttp(entities as IEnumerable<Role>) : Result.Failure<IEnumerable<Role>>(DBErrors.Repository.Entity.NotFound);
        }, cancellationToken);

    }

}
