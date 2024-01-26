using Common.Infrastructure.Communication.Messages;
using Members.Application;
using Microsoft.Extensions.DependencyInjection;
using SwarmBot.Infrastructure.Communication;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using TGF.CA.Infrastructure.Communication.Messages.Discord;
using TGF.CA.Infrastructure.Communication.Publisher.Integration;
using TGF.Common.ROP.HttpResult;

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
            var lMembersService = lScope.ServiceProvider.GetRequiredService<IMembersService>();

            await lMembersService.RevokeMemberRoleList(ulong.Parse(aIntegrationMessage.Content.DiscordUserId), aIntegrationMessage.Content.DiscordRoleList, aCancellationToken)
            .Tap(isPermissionsChanged =>
            {
                if (isPermissionsChanged)
                    lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>()
                        .Publish(new MemberTokenRevoked([aIntegrationMessage.Content.DiscordUserId]), routingKey: RoutingKeys.Members.Member_revoke);
            });
        }
    }
}
