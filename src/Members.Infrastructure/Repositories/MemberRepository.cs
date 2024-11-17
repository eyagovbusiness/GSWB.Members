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

        public async Task<IHttpResult<IEnumerable<Member>>> GetByIdListAsync(
            IEnumerable<MemberKey> memberIdList,
            ISpecification<Member>? specification,
            CancellationToken cancellationToken = default)
        {
            return await TryQueryAsync(async cancellationToken =>
            {
                var entityIdList = memberIdList.ToList();
                if (entityIdList.Count == 0)
                    return Result.SuccessHttp(Enumerable.Empty<Member>());

                // Extract the GuildId and UserId pairs into a list
                var guildIds = entityIdList.Select(e => e.GuildId).ToList();
                var userIds = entityIdList.Select(e => e.UserId).ToList();

                // Ensure query logic is database-compatible
                IQueryable<Member> query = Queryable.Where(entity =>
                    guildIds.Contains(entity.GuildId) &&
                    userIds.Contains(entity.UserId));

                // Apply specification if available
                if (specification != null)
                {
                    query = _specificationEvaluator.GetQuery(query, specification);
                }

                var entities = await query.ToListAsync(cancellationToken);

                return entities.Count != 0
                    ? Result.SuccessHttp(entities as IEnumerable<Member>)
                    : Result.Failure<IEnumerable<Member>>(DBErrors.Repository.Entity.NotFound);
            }, cancellationToken);
        }


    }

}
