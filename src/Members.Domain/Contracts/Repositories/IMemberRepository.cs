using Ardalis.Specification;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using TGF.CA.Domain.Contracts.Repositories;
using TGF.Common.ROP.HttpResult;

namespace Members.Domain.Contracts.Repositories
{
    /// <summary>
    /// Provides an interface for repository operations related to the <see cref="Member"/> entity.
    /// </summary>
    public interface IMemberRepository : IRepository<Member>
    {
        /// <summary>
        /// Retrieves a Member by its Id asynchronously.
        /// </summary>
        /// <param name="memberId">The identifier of the member.</param>
        /// <param name="aCancellationToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing the result of retrieving the entity.</returns>
        public Task<IHttpResult<Member>> GetByIdAsync(MemberKey memberId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a Member by its Id asynchronously.
        /// </summary>
        /// <param name="memberId">The identifier of the member.</param>
        /// <param name="aCancellationToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing the result of retrieving the entity.</returns>
        Task<IHttpResult<Member>> GetByIdAsync(MemberKey memberId, ISpecification<Member> specification, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Retrieves a list of members by its identifiers asynchronously.
        /// </summary>
        /// <param name="memberIdList">The list IDs used to query the members.</param>
        /// <param name="aCancellationToken">Cancellation token to cancel the query if needed.</param>
        public Task<IHttpResult<IEnumerable<Member>>> GetByIdListAsync(IEnumerable<MemberKey> entityIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of members by its identifiers asynchronously.
        /// </summary>
        /// <param name="memberIdList">The list IDs used to query the members.</param>
        /// <param name="aCancellationToken">Cancellation token to cancel the query if needed.</param>
        Task<IHttpResult<IEnumerable<Member>>> GetByIdListAsync(IEnumerable<MemberKey> memberIdList, ISpecification<Member> specification, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Deletes a specified member from the repository.
        /// </summary>
        /// <param name="aMemberToDelete">The member to delete.</param>
        /// <returns>The deleted member or Error</returns>
        Task<IHttpResult<Member>> Delete(Member aMemberToDelete, CancellationToken aCancellationToken = default);

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