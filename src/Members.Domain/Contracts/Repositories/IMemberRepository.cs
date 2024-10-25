using Members.Domain.Entities;
using TGF.CA.Domain.Contracts.Repositories;
using TGF.Common.ROP.HttpResult;

namespace Members.Domain.Contracts.Repositories
{
    /// <summary>
    /// Provides an interface for repository operations related to the <see cref="Member"/> entity.
    /// </summary>
    public interface IMemberRepository : IRepositoryBase<Member, Guid>
    {

        /// <summary>
        /// Deletes a specified member from the repository.
        /// </summary>
        /// <param name="aMemberToDelete">The member to delete.</param>
        /// <returns>The deleted member or Error</returns>
        Task<IHttpResult<Member>> Delete(Member aMemberToDelete, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Retrieves a member by their Discord user ID.
        /// </summary>
        /// <param name="aDiscordUserId">The Discord user ID to search for.</param>
        /// <returns>The member matching the given Id or Error.</returns>
        Task<IHttpResult<Member>> GetByDiscordUserIdAsync(Guid id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get a member identified by the GuildId and the UserId.
        /// </summary>
        /// <returns>The member matching the given GuildId and UserId or Error.</returns>
        Task<IHttpResult<Member>> GetByGuildAndUserIdsAsync(ulong guildId, ulong userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of members.
        /// </summary>
        /// <param name="aPageSize">The number of members to retrieve per page.</param>
        /// <param name="aPageNumber">The page number to retrieve. Default is 1.</param>
        /// <returns>The list of members matching the filters or Error.</returns>
        Task<IHttpResult<IEnumerable<Member>>> GetMembersListAsync(
            int aPage, int aPageSize,
            string aSortBy,
            string? aDiscordNameFilter = null, string? aGameHandleFilter = null, ulong? aRoleIdFilter = null, bool? aIsVerifiedFilter = null,
            CancellationToken aCancellationToken = default);

        /// <summary>
        /// Updates a specified member in the repository.
        /// </summary>
        /// <param name="aMember">The member to update.</param>
        /// <returns>The updated member or Error.</returns>
        Task<IHttpResult<Member>> Update(Member aMember, CancellationToken aCancellationToken = default);

    }

}