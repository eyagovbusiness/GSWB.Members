﻿using Common.Infrastructure.Communication.Messages;
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
    internal class MemberRoleAssignedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberRoleAssigned>
    {
        public async Task Handle(IntegrationMessage<MemberRoleAssigned> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var lMembersService = lScope.ServiceProvider.GetRequiredService<IMembersService>();

            await lMembersService.AssignMemberRoleList(ulong.Parse(aIntegrationMessage.Content.UserId), ulong.Parse(aIntegrationMessage.Content.GuildId), aIntegrationMessage.Content.DiscordRoleList, aCancellationToken)
            .Tap(updateMemberRolesResultDto =>
            {
                if (updateMemberRolesResultDto.IsPermissionsChanged)
                    lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>()
                        .Publish(new MemberTokenRevoked([updateMemberRolesResultDto.Member.Id]), routingKey: RoutingKeys.Members.Member_revoke);
            });
        }
    }
}
