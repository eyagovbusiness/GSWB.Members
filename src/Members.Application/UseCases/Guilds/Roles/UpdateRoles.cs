using Common.Application.Contracts.Communication;
using Common.Application.Contracts.Communication.Messages;
using Common.Application.DTOs.Roles;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.ValueObjects.Role;
using TGF.CA.Application.Contracts.Communication;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Guilds.Roles
{
    /// <summary>
    /// Use case to update roles with changes in application fields.
    /// </summary>
    /// <remarks>This use case does not support upating discord fields of the role, for that there is a dedicated use case(see <see cref="UpdateRoleDiscordData"/>)</remarks>
    public class UpdateRoles(IGuildRepository guildRepository, IIntegrationMessagePublisher integrationMessagePublisher)
        : IUseCase<IHttpResult<IEnumerable<RoleDTO>>, GuildRolesUpdateDTO>
    {
        public async Task<IHttpResult<IEnumerable<RoleDTO>>> ExecuteAsync(GuildRolesUpdateDTO request, CancellationToken cancellationToken = default)
        => await guildRepository.GetGuildWithRoles(ulong.Parse(request.GuildId), cancellationToken)
        .Bind(guild => 
            guild.UpdateRoles(request.RoleUpdates
                .Select(roleUpdate => 
                    new RoleUpdate(
                    roleUpdate.Permissions,
                    roleUpdate.Type,
                    roleUpdate.Description,
                    ulong.Parse(roleUpdate.Id))
                )
            )
        )
        .Bind(guild => guildRepository.UpdateAsync(guild))
        .Map(guild => guild.Roles.Select(role => role.ToDto()))
        .Tap(roleDTOList =>
        {
            var lUpdatedRoleIdList = roleDTOList.Select(roleDTO => ulong.Parse(roleDTO.Id));
            integrationMessagePublisher.Publish(new RoleTokenRevoked(lUpdatedRoleIdList.ToArray()), routingKey: RoutingKeys.Members.Member_role_revoke);
        });
    }
}
