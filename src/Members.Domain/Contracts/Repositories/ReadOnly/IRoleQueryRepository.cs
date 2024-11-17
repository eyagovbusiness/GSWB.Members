using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using TGF.CA.Domain.Contracts.Repositories;
using TGF.Common.ROP.HttpResult;

namespace Members.Domain.Contracts.Repositories.ReadOnly
{
    public interface IRoleQueryRepository : IQueryRepository<Role>
    {
        /// <summary>
        /// Retrieves a single role by its composite key (RoleId and GuildId).
        /// </summary>
        Task<IHttpResult<Role>> GetByIdAsync(RoleKey roleKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of roles by a list of composite keys (RoleId and GuildId).
        /// </summary>
        Task<IHttpResult<IEnumerable<Role>>> GetByIdListAsync(IEnumerable<RoleKey> roleKeys, CancellationToken cancellationToken = default);

    }
}
