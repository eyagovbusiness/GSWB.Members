using Common.Application.Contracts.Communication.Messages.Discord;
using Common.Application.DTOs.Roles;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.ValueObjects.Role;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Guilds.Roles
{
    /// <summary>
    /// Use case to create a new role
    /// </summary>
    public class CreateRoles(IGuildRepository guildRepository) : IUseCase<IHttpResult<RoleDTO>, RoleCreated>
    {
        public async Task<IHttpResult<RoleDTO>> ExecuteAsync(RoleCreated request, CancellationToken cancellationToken = default)
        => await guildRepository.GetByIdAsync(ulong.Parse(request.GuildId), cancellationToken)
        .Bind(guild => guild.AddRoles([new DiscordRoleValues(ulong.Parse(request.DiscordRole.RoleId), request.DiscordRole.Name, request.DiscordRole.Position)]))
        .Map(guild => guild.Roles.First(role => role.Id == ulong.Parse(request.DiscordRole.RoleId)).ToDto());
    }
}
