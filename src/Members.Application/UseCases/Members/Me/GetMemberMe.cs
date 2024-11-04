using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members.Me
{
    /// <summary>
    /// Get the member me(the member associated to the currently logged user).
    /// </summary>
    public class GetMemberMe(
        IMemberRepository memberRepository,
        IRoleQueryRepository roleQueryRepository
    )
        : IUseCase<IHttpResult<MemberDetailDTO>, GuildAndUserId>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(GuildAndUserId request, CancellationToken cancellationToken = default)
        {
            Member lMember = default!;
            return await memberRepository.GetByGuildAndUserIdsAsync(request.GuildId, request.UserId, cancellationToken)
            .Tap(member => lMember = member)
            .Bind(member => roleQueryRepository.GetByIdListAsync(member.Roles.Select(memberRole => memberRole.RoleId)))
            .Map(roles => lMember.ToDetailDto(roles, aIncludeDiscordOnlyRoles: false));
        }
    }
}
