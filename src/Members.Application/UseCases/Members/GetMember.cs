using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use case to get a <see cref="MemberDTO"/>
    /// </summary>
    /// <param name="memberRepository"></param>
    public class GetMember(
        IMemberRepository memberRepository
    )
         : IUseCase<IHttpResult<MemberDTO>, MemberKey>
    {
        public Task<IHttpResult<MemberDTO>> ExecuteAsync(MemberKey request, CancellationToken cancellationToken = default)
        => memberRepository.GetByGuildAndUserIdsAsync(request.GuildId, request.UserId, cancellationToken)
            .Map(member => member.ToDto());
    }
}
