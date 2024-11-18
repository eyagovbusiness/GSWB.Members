using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Contracts.Services;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
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
        IGameVerificationService gameVerificationService
    )
        : IUseCase<IHttpResult<MemberDetailDTO>, MemberKey>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(MemberKey request, CancellationToken cancellationToken = default)
        {
            var lMemberResult = await memberRepository.GetByIdAsync(request, cancellationToken);
            return await lMemberResult.Verify(member => member?.GameHandle != null, ApplicationErrors.MemberValidation.GameHandleNotSet)
                .Verify(member => member!.VerificationCodeExpiryDate > DateTimeOffset.Now, ApplicationErrors.MemberValidation.GameHandleVerificationCodeExpired)
                .Bind(member => gameVerificationService.VerifyGameHandle(member!.GameHandle!, member.GameHandleVerificationCode, cancellationToken))
                .Bind(_ => UpdateIsVerified(lMemberResult.Value!, cancellationToken));
        }
        private async Task<IHttpResult<MemberDetailDTO>> UpdateIsVerified(Member aMember, CancellationToken aCancellationToken = default)
        => aMember.Verify()
        ? await memberRepository.UpdateAsync(aMember, aCancellationToken)
            .Map(member => member.ToDetailDto())
        : Result.Failure<MemberDetailDTO>(ApplicationErrors.MemberValidation.GameHandleVerificationFailed);

    }
}
