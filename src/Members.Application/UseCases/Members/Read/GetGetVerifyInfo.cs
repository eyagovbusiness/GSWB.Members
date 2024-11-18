using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Specifications.With;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;

namespace Members.Application.UseCases.Members.Read
{
    /// <summary>
    /// Use Case to get the game handle verification info related data for a given member by its Id as <see cref="MemberVerificationStateDTO"/>.
    /// </summary>
    public class GetGetVerifyInfo(
        IMemberRepository memberRepository
        )
        : IUseCase<IHttpResult<MemberVerificationStateDTO>, MemberKey>
    {
        public async Task<IHttpResult<MemberVerificationStateDTO>> ExecuteAsync(MemberKey request, CancellationToken cancellationToken = default)
        => await memberRepository.GetByIdAsync(request, new MemberWithVerifyCodeSpec(), cancellationToken)
        .Bind(member => member.EnsureGameHandleVerificationCode())
        .Bind(member => TryUpdateGameHandleVerifyCode(member!, cancellationToken))
        .Map(member => new MemberVerificationStateDTO(member.IsGameHandleVerified, member!.VerifyCode!.Code, member.VerifyCode!.ExpiryDate));

        private async Task<IHttpResult<Member>> TryUpdateGameHandleVerifyCode(Member aMember, CancellationToken aCancellationToken = default)
        => await aMember.TryRefreshGameHandleVerificationCode()
        .Bind(member => memberRepository.UpdateAsync(aMember, aCancellationToken));

    }
}
