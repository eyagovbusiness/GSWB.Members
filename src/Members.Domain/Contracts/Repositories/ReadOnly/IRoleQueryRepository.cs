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
        /// <param name="roleId">The RoleId.</param>
        /// <param name="guildId">The GuildId.</param>
        /// <returns>The role if found.</returns>
        Task<IHttpResult<Role>> GetByIdAsync(ulong roleId, ulong guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of roles by a list of composite keys (RoleId and GuildId).
        /// </summary>
        /// <param name="roleIds">A list of tuples containing RoleId and GuildId for each role.</param>
        /// <returns>A list of roles matching the provided composite keys.</returns>
        Task<IHttpResult<IEnumerable<Role>>> GetByIdListAsync(IEnumerable<(ulong RoleId, ulong GuildId)> roleIds, CancellationToken cancellationToken = default);
    }
}
