using Members.Domain.Entities;
using TGF.CA.Domain.Contracts.Repositories.EntityRepository;
using TGF.Common.ROP.HttpResult;

namespace Members.Domain.Contracts.Repositories
{
    public interface IGuildRepository : IEntityRepository<Guild, ulong>
    {
        Task<IHttpResult<Guild>> GetGuildWithRoles(ulong guildId, CancellationToken cancellationToken = default);
    }
}
