using Ardalis.Specification;
using Common.Domain.ValueObjects;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Members.Infrastructure.DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB;
using TGF.CA.Infrastructure.DB.Repository;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Infrastructure.Repositories
{
    internal class MemberRepository(MembersDbContext aContext, ILogger<MemberRepository> aLogger)
        : Repository<MemberRepository, MembersDbContext, Member>(aContext, aLogger), IMemberRepository, ISortRepository
    {

        public virtual async Task<IHttpResult<Member>> GetByIdAsync(MemberKey memberId, CancellationToken cancellationToken = default)
        => await GetByIdAsync(memberId, null, cancellationToken);
        public async Task<IHttpResult<Member>> GetByIdAsync(MemberKey memberId, ISpecification<Member>? specification, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async cancellationToken =>
        {
            var entity = specification != null
            ? await _specificationEvaluator.GetQuery(Queryable, specification).FirstOrDefaultAsync(e => e.GuildId == memberId.GuildId && e.UserId == memberId.UserId, cancellationToken)
            : await Queryable.FirstOrDefaultAsync(e => e.GuildId == memberId.GuildId && e.UserId == memberId.UserId, cancellationToken);
            return entity != null ? Result.SuccessHttp(entity!) : Result.Failure<Member>(DBErrors.Repository.Entity.NotFound);
        }, cancellationToken);

        public virtual async Task<IHttpResult<IEnumerable<Member>>> GetByIdListAsync(IEnumerable<MemberKey> entityIds, CancellationToken cancellationToken = default)
        => await GetByIdListAsync(entityIds, null, cancellationToken);

        public async Task<IHttpResult<IEnumerable<Member>>> GetByIdListAsync(IEnumerable<MemberKey> memberIdList, ISpecification<Member>? specification, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async cancellationToken =>
        {
            var entityIdList = memberIdList.ToList();
            if (entityIdList.Count == 0) return Result.SuccessHttp(Enumerable.Empty<Member>());



            var entities = specification != null
            ? await _specificationEvaluator.GetQuery(Queryable, specification)
                .Where(entitiy => entityIdList
                    .Any(memberKey => memberKey.GuildId == entitiy.GuildId
                        && memberKey.UserId == entitiy.UserId)
                )
                .ToListAsync(cancellationToken)
            : await Queryable
                .Where(entitiy => entityIdList
                    .Any(memberKey => memberKey.GuildId == entitiy.GuildId
                        && memberKey.UserId == entitiy.UserId)
                )
                .ToListAsync(cancellationToken);

            return entities.Count != 0 ? Result.SuccessHttp(entities as IEnumerable<Member>) : Result.Failure<IEnumerable<Member>>(DBErrors.Repository.Entity.NotFound);
        }, cancellationToken);

        public async Task<IHttpResult<IEnumerable<Member>>> GetMembersListAsync(
            int aPage, int aPageSize,
            string aSortBy,
            string? aDiscordNameFilter = null, string? aGameHandleFilter = null, ulong? aRoleIdFilter = null, bool? aIsVerifiedFilter = null,
            CancellationToken aCancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken) =>
        {
            var lQuery = _context.Members
            .Include(m => m.Roles)
            .AsQueryable();

            lQuery = ApplyFilters(lQuery, aDiscordNameFilter, aGameHandleFilter, aRoleIdFilter, aIsVerifiedFilter);
            lQuery = ISortRepository.ApplySorting(lQuery, aSortBy);

            return await lQuery
                .Skip((aPage - 1) * aPageSize)
                .Take(aPageSize)
                .ToListAsync(aCancellationToken) as IEnumerable<Member>;
        }, aCancellationToken);


        public async Task<IHttpResult<Member>> Update(Member aMember, CancellationToken aCancellationToken = default)
            => await TryCommandAsync(() => _context.Members.Update(aMember).Entity, aCancellationToken: aCancellationToken);

        public async Task<IHttpResult<Member>> Delete(Member aMemberToDelete, CancellationToken aCancellationToken = default)
            => await TryCommandAsync(() => _context.Members.Remove(aMemberToDelete).Entity, aCancellationToken: aCancellationToken);

        #region Private

        private static IQueryable<Member> ApplyFilters(
            IQueryable<Member> aQuery,
            string? aDiscordNameFilter,
            string? aGameHandleFilter,
            ulong? aRoleIdFilter,
            bool? aIsVerifiedFilter)
        {
            if (!string.IsNullOrEmpty(aDiscordNameFilter))
            {
                var lLowercaseDiscordNameFilter = $"%{aDiscordNameFilter.ToLowerInvariant()}%";
                aQuery = aQuery.Where(m => EF.Functions.Like(m.DiscordGuildDisplayName.ToLower(), lLowercaseDiscordNameFilter));
            }

            if (!string.IsNullOrEmpty(aGameHandleFilter))
            {
                var lLowercaseGameHandleFilter = $"%{aGameHandleFilter.ToLowerInvariant()}%";
                aQuery = aQuery.Where(m => !string.IsNullOrWhiteSpace(m.GameHandle) && EF.Functions.Like(m.GameHandle.ToLower(), lLowercaseGameHandleFilter));
            }

            if (aRoleIdFilter.HasValue)
                aQuery = aQuery.Where(m => m.Roles.Any(r => r.RoleId == aRoleIdFilter.Value));

            if (aIsVerifiedFilter.HasValue)
                aQuery = aQuery.Where(m => m.IsGameHandleVerified == aIsVerifiedFilter.Value);

            return aQuery;
        }


        #endregion

    }

}
