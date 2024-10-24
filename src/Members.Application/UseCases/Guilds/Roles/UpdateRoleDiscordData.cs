﻿using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Roles;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.ValueObjects.Role;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Guilds.Roles
{
    /// <summary>
    /// Use case to update a given role with new discord data(position or name)
    /// </summary>
    /// <remarks>This use case does not support upating application(not discord related) fields, for that there is a dedicated use case(see <see cref="UpdateRoles"/>)</remarks>
    public class UpdateRoleDiscordData(IGuildRepository guildRepository)
        : IUseCase<IHttpResult<IEnumerable<RoleDTO>>, DiscordRoleDTO>
    {
        public async Task<IHttpResult<IEnumerable<RoleDTO>>> ExecuteAsync(DiscordRoleDTO request, CancellationToken cancellationToken = default)
        => await guildRepository.GetGuildWithRoles(ulong.Parse(request.GuildId), cancellationToken)
        .Bind(guild =>
            guild.UpdateRoles(
                    [new DiscordRoleValues(
                    ulong.Parse(request.RoleId),
                    request.Name,
                    request.Position)]
            ))
        .Map(guild => guild.Roles.Select(role => role.ToDto()));
    }
}