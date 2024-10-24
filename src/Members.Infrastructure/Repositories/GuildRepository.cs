using Ardalis.Specification;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB.Repository;
using TGF.Common.ROP.HttpResult;

namespace Members.Infrastructure.Repositories
{
    internal class GuildRepository(MembersDbContext aContext, ILogger<GuildRepository> aLogger)
        : RepositoryBase<GuildRepository, MembersDbContext, Guild, ulong>(aContext, aLogger), IGuildRepository, ISortRepository
    {

        public async Task<IHttpResult<Guild>> GetGuildWithRoles(ulong guildId, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken) =>
        {
            return (await _context.Guilds
            .Include(e => e.Roles)
            .FirstOrDefaultAsync(cancellationToken: aCancellationToken))!;

        }, cancellationToken)
        .Verify(guild => guild != null!, InfrastructureErrors.GuildsDb.NotFoundId);
    }

}
