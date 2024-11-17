using Ardalis.Specification;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
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


    }

}