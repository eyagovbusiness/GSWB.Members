using Common.Application.Contracts.Communication;
using Common.Application.Contracts.Communication.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Application.Specifications.With;
using Members.Domain.Contracts.Repositories;
using TGF.CA.Application.Contracts.Communication;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Guilds.Roles
{
    /// <summary>
    /// Use case to delete a role.
    /// </summary>
    public class DeleteRole(IGuildRepository guildRepository, IIntegrationMessagePublisher integrationMessagePublisher)
        : IUseCase<IHttpResult<IEnumerable<RoleDTO>>, RoleDeleted>
    {
        public async Task<IHttpResult<IEnumerable<RoleDTO>>> ExecuteAsync(RoleDeleted request, CancellationToken cancellationToken = default)
        => await guildRepository.GetByIdAsync(ulong.Parse(request.GuildId), new GuildWithRolesSpec(), cancellationToken)
        .Bind(guild => guild.DeleteRoles([ulong.Parse(request.RoleId)]))
        .Bind(guild => guildRepository.UpdateAsync(guild,cancellationToken))
        .Map(guild => guild.Roles.Select(role => role.ToDto()))
        .Tap(roleDTOList =>
        {
            integrationMessagePublisher.Publish(new RoleTokenRevoked([new RoleKey(request.GuildId,request.RoleId)]), routingKey: RoutingKeys.Members.Member_role_revoke);
        });

    }
}
