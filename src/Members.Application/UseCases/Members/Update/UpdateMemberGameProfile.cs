using Common.Application.DTOs.Members;
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
    /// Use case to update the game profile details of the member.
    /// </summary>
    public class UpdateMemberGameProfile(
        IMemberRepository memberRepository
    )
        : IUseCase<IHttpResult<MemberDetailDTO>, MemberGameProfileUpdate>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(MemberGameProfileUpdate request, CancellationToken cancellationToken = default)
        => await memberRepository.GetByIdAsync(request.Id, cancellationToken)
            .Bind(member => UpdateGameProfile(member!, request, cancellationToken))
            .Map(member => member.ToDetailDto());

        private async Task<IHttpResult<Member>> UpdateGameProfile(Member aMember, MemberGameProfileUpdate aMemberProfileDTO, CancellationToken aCancellationToken = default)
        => aMember.UpdateProfile(aMemberProfileDTO.GameHandle, aMemberProfileDTO.SpectrumCommunityMoniker)
            ? await memberRepository.UpdateAsync(aMember, aCancellationToken)
            : Result.SuccessHttp(aMember);

    }
}
