﻿using Common.Application.DTOs.Members;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Members.Domain.Services;
using Members.Domain.ValueObjects;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use case to revoke a given list of roles for a guild member.
    /// </summary>
    public class RevokeMemberRoles(
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

            return await memberRepository.GetByGuildAndUserIdsAsync(ulong.Parse(request.GuildId), ulong.Parse(request.UserId), cancellationToken)
                .Tap(member => lMember = member)
                .Bind(member => guildRepository.GetGuildWithRoles(member.GuildId, cancellationToken))
                .Tap(guild => lGuild = guild)
                .Bind(guild => guildMemberRoleService.RevokeRoles(guild, lMember, roleIdList))
                .Tap(updateResult => memberRolesUpdateResult = updateResult)
                .Bind(updateResult => memberRepository.UpdateAsync(updateResult.Member, cancellationToken))
                .Map(updatedMember => new MemberRolesUpdateResult(updatedMember, memberRolesUpdateResult.IsPermissionsChanged))
                .Map(updatedMemberRolesUpdateResult =>
                {
                    var memberRoleIds = lMember.Roles.Select(memberRole => memberRole.RoleId).ToArray();
                    return updatedMemberRolesUpdateResult.ToDto(lGuild.Roles.Where(role => memberRoleIds.Contains(role.Id)));
                });
        }
    }
}
