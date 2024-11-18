using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Contracts.Services;
using Members.Application.Mapping;
using Members.Application.Specifications.With;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Members.Domain.Validation.Member;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;

namespace Members.Application.UseCases.Members.Update
{
    /// <summary>
    /// Use Case to update the member's status.
    /// </summary>
    public class VerifyGameHandle(
        IMemberRepository memberRepository,
        IGameVerificationService gameVerificationService,
        MemberVerifyCodeValidor validationRules
    )
        : IUseCase<IHttpResult<MemberDetailDTO>, MemberKey>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(MemberKey request, CancellationToken cancellationToken = default)
        {
            var lMemberResult = await memberRepository.GetByIdAsync(request, new MemberWithVerifyCodeSpec(), cancellationToken);
            return await lMemberResult.Validate(lMemberResult.Value, validationRules)
                .Bind(member => gameVerificationService.VerifyGameHandle(member!.GameHandle!, member.VerifyCode!.Code, cancellationToken))
                .Bind(_ => UpdateIsVerified(lMemberResult.Value!, cancellationToken));
        }
        private async Task<IHttpResult<MemberDetailDTO>> UpdateIsVerified(Member member, CancellationToken cancellationToken = default)
        => await member.Verify()
        .Bind(member => memberRepository.UpdateAsync(member, cancellationToken))
        .Map(member => member.ToDetailDto());

    }
}
