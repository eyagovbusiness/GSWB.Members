using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Members.Domain.Validation;
using TGF.CA.Application.Services;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.Services
{
    public class RefreshMemberVerifyCodeService(IMemberRepository aMemberRepository, MemberVerificationValidator aMemberVerificationValidator)
        : IApplicationService
    {
        public Task<IHttpResult<Member>> RefreshMemberVerifyCodeAsync(Guid aMemberId, CancellationToken aCancellationToken)
        => aMemberRepository.GetByIdAsync(aMemberId, aCancellationToken)
            .Bind(member => member.RefreshVerifyCode(aMemberVerificationValidator))
            .Bind(member => aMemberRepository.UpdateAsync(member, aCancellationToken));
    }
}
