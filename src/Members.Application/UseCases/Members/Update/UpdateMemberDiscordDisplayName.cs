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
    /// Use Case to update the member's discord display name.
    /// </summary>
    public class UpdateMemberDiscordDisplayName(
        IMemberRepository memberRepository
    )
        : IUseCase<IHttpResult<MemberDetailDTO>, MemberDiscordDisplayName>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(MemberDiscordDisplayName request, CancellationToken cancellationToken = default)
        => await memberRepository.GetByIdAsync(request.Id, cancellationToken)
            .Bind(member => UpdateMemberDisplayName(member!, request.DisplayName, cancellationToken))
            .Map(member => member.ToDetailDto());

        private async Task<IHttpResult<Member>> UpdateMemberDisplayName(Member aMember, string aNewDisplayName, CancellationToken aCancellationToken = default)
        {
            aMember.DiscordGuildDisplayName = aNewDisplayName;
            return await memberRepository.UpdateAsync(aMember, aCancellationToken);
        }

    }
}
