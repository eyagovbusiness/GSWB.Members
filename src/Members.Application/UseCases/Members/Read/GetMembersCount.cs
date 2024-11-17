using Members.Domain.Contracts.Repositories;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Members.Read
{
    public class GetMembersCount(
        IMemberRepository memberRepository
        )
        : IUseCase<IHttpResult<int>, Unit>
    {
        public async Task<IHttpResult<int>> ExecuteAsync(Unit request, CancellationToken cancellationToken = default)
        => await memberRepository.GetCountAsync(cancellationToken);
    }
}
