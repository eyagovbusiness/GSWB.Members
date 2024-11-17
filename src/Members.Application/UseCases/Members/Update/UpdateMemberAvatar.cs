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
    /// Use Case to update the member's Avatar.
    /// </summary>
    public class UpdateMemberAvatar(
        IMemberRepository memberRepository
    )
        : IUseCase<IHttpResult<MemberDetailDTO>, MemberAvatar>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(MemberAvatar request, CancellationToken cancellationToken = default)
        => await memberRepository.GetByIdAsync(request.Id, cancellationToken)
            .Bind(member => UpdateAvatar(member!, request.AvatarUrl, cancellationToken))
            .Map(member => member.ToDetailDto());

        private async Task<IHttpResult<Member>> UpdateAvatar(Member aMember, string aNewAvatarUrl, CancellationToken aCancellationToken = default)
        {
            aMember.DiscordAvatarUrl = aNewAvatarUrl;
            return await memberRepository.UpdateAsync(aMember, aCancellationToken);
        }

    }
}
