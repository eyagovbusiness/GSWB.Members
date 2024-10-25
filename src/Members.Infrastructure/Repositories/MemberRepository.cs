using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB.Repository;
using TGF.Common.ROP.HttpResult;

namespace Members.Infrastructure.Repositories
{
    internal class MemberRepository(MembersDbContext aContext, ILogger<MemberRepository> aLogger)
        : RepositoryBase<MemberRepository, MembersDbContext, Member, Guid>(aContext, aLogger), IMemberRepository, ISortRepository
    {
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

        public async Task<IHttpResult<Member>> GetByDiscordUserIdAsync(Guid Id, CancellationToken aCancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken) =>
        {
            return await _context.Members
            .Include(m => m.Roles)
            .SingleOrDefaultAsync(m => m.Id == Id, aCancellationToken);
        }, aCancellationToken)
        .Verify(member => member! != null!, InfrastructureErrors.MembersDb.NotFoundDiscordUserId)
        .Map(member => member!);

        public async Task<IHttpResult<Member>> GetByGuildAndUserIdsAsync(ulong guildId, ulong userId, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken) =>
        {
            return await _context.Members
            .Include(m => m.Roles)
            .SingleOrDefaultAsync(m => m.UserId == userId && m.GuildId == guildId, aCancellationToken);
        }, cancellationToken)
        .Verify(member => member! != null!, InfrastructureErrors.MembersDb.NotFoundDiscordUserId)
        .Map(member => member!);

        public async Task<IHttpResult<Member>> Update(Member aMember, CancellationToken aCancellationToken = default)
            => await TryCommandAsync(() => _context.Members.Update(aMember).Entity, aCancellationToken);

        public async Task<IHttpResult<Member>> Delete(Member aMemberToDelete, CancellationToken aCancellationToken = default)
            => await TryCommandAsync(() => _context.Members.Remove(aMemberToDelete).Entity, aCancellationToken);

        public override async Task<IHttpResult<Member>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken) =>
        {
            return await _context.Members
            .Include(m => m.Roles)
            .SingleOrDefaultAsync(m => m.Id == id, aCancellationToken);
        }, cancellationToken)
        .Verify(member => member! != null!, InfrastructureErrors.MembersDb.NotFoundId)
        .Map(member => member!);

        public override async Task<IHttpResult<IEnumerable<Member>>> GetByIdListAsync(IEnumerable<Guid> aMemberIdList, CancellationToken aCancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken) =>
        {
            var lMemberList = await _context.Members
                .Include(m => m.Roles)
                .Where(m => aMemberIdList.Contains(m.Id))
                .ToListAsync(aCancellationToken);

            return lMemberList;
        }, aCancellationToken)
        .Verify(members => members != null && members.Count == aMemberIdList.Count(), InfrastructureErrors.MembersDb.NotFoundIdList)
        .Map(members => members!.AsEnumerable());

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
