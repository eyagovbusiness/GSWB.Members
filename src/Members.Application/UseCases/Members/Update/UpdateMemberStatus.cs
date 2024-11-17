using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Members.Domain.ValueObjects;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;

namespace Members.Application.UseCases.Members.Update
{
    /// <summary>
    /// Use Case to update the member's status.
    /// </summary>
    public class UpdateMemberStatus(
        IMemberRepository memberRepository
    )
        : IUseCase<IHttpResult<MemberDetailDTO>, MemberStatus>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(MemberStatus request, CancellationToken cancellationToken = default)
        => await memberRepository.GetByIdAsync(request.Id, cancellationToken)
            .Bind(member => UpdateStatus(member!, request.Status, cancellationToken))
            .Map(member => member.ToDetailDto());

        private async Task<IHttpResult<Member>> UpdateStatus(Member aMember, MemberStatusEnum aMemberStatus, CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResult(aCancellationToken)
            .Tap(_ => aMember!.Status = aMemberStatus)
            .Bind(member => memberRepository.UpdateAsync(aMember, aCancellationToken));

    }
}
