using Common.Application.DTOs.Members;
using Members.Application.Services;
using Members.Domain.Contracts.Repositories;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Member
{
    public class GetMemberVerificationStateUseCase(IMemberRepository aMemberRepository, RefreshMemberVerifyCodeService aRefreshMemberVerifyCodeService)
        : IUseCase<IHttpResult<MemberVerificationStateDTO>, Guid>
    {
        public async Task<IHttpResult<MemberVerificationStateDTO>> ExecuteAsync(Guid aMemberId, CancellationToken aCancellationToken = default)
        {
            var lMember = await aMemberRepository.GetByIdAsync(aMemberId, aCancellationToken);

            if (lMember.IsSuccess && lMember.Value.VerifyCode?.ExpiryDate <= DateTimeOffset.Now)
                lMember = await aRefreshMemberVerifyCodeService.RefreshMemberVerifyCodeAsync(lMember.Value.Id, aCancellationToken)
                    .Bind(member => aMemberRepository.UpdateAsync(member, aCancellationToken));

            return lMember.Map(member => new MemberVerificationStateDTO(member.IsGameHandleVerified, member.VerifyCode!.Code, member.VerifyCode!.ExpiryDate));
        }
    }
}
