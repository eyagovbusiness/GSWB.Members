using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Application.Specifications.With;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Members.Domain.Services;
using Members.Domain.ValueObjects;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use case to assign a given list of roles to a guild member.
    /// </summary>
    public class AssignMemberRoles(
        IMemberRepository memberRepository,
        IGuildRepository guildRepository,
        GuildMemberRoleService guildMemberRoleService
    )
        : IUseCase<IHttpResult<MemberRolesUpdateResultDTO>, MemberRolesDTO>
    {
        public async Task<IHttpResult<MemberRolesUpdateResultDTO>> ExecuteAsync(MemberRolesDTO request, CancellationToken cancellationToken = default)
        {
            var roleIdList = request.RoleIdList.Select(ulong.Parse);

            Member lMember = default!;
            Guild lGuild = default!;
            MemberRolesUpdateResult memberRolesUpdateResult = default!;

            return await memberRepository.GetByIdAsync(new MemberKey(request.GuildId, request.UserId), cancellationToken)
                .Tap(member => lMember = member)
                .Bind(member => guildRepository.GetByIdAsync(member.GuildId, new GuildWithRolesSpec(), cancellationToken))
                .Tap(guild => lGuild = guild)
                .Bind(guild => guildMemberRoleService.AssignRoles(guild, lMember, roleIdList))
                .Tap(updateResult => memberRolesUpdateResult = updateResult)
                .Bind(updateResult => memberRepository.UpdateAsync(updateResult.Member, cancellationToken))
                .Map(updatedMember => new MemberRolesUpdateResult(updatedMember, memberRolesUpdateResult.IsPermissionsChanged))
                .Map(updatedMemberRolesUpdateResult =>
                {
                    var memberRoleIds = lMember.Roles.Select(memberRole => memberRole.RoleId).ToArray();
                    return updatedMemberRolesUpdateResult.ToDto(lGuild.Roles.Where(role => memberRoleIds.Contains(role.RoleId)));
                });
        }
    }
}
