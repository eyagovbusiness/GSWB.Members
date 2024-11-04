using Common.Application.Contracts.Communication;
using Common.Application.DTOs.Members;
using Common.Application.Contracts.Communication.Messages;
using Members.Application.UseCases.Members;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.CA.Application.Contracts.Communication;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    /// <summary>
    /// <see cref="MemberRoleRevoked"/> message handler. 
    /// Revokes certain roles for a given member and if the member's highest role is between the revoked ones then revokes all tokens for this member, since permissions for this member(given my its highest role assigned) changed. 
    /// This forces the client to get a new token pair with the latest permissions.
    /// </summary>
    internal class MemberRoleRevokedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberRoleRevoked>
    {
        public async Task Handle(IntegrationMessage<MemberRoleRevoked> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var revokeMemberRolesUseCase = lScope.ServiceProvider.GetRequiredService<RevokeMemberRoles>();

            await revokeMemberRolesUseCase.ExecuteAsync(new MemberRolesDTO(aIntegrationMessage.Content.GuildId, aIntegrationMessage.Content.UserId, aIntegrationMessage.Content.DiscordRoleList.Select(x => x.RoleId)), aCancellationToken)
            .Tap(updateResult =>
            {
                if (updateResult.IsPermissionsChanged)
                    lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>()
                        .Publish(new MemberTokenRevoked([updateResult.Member.Id]), routingKey: RoutingKeys.Members.Member_revoke);
            });
        }
    }
}
