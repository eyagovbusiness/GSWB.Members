using Common.Application.Contracts.Communication;
using Common.Domain.ValueObjects;
using Common.Application.Contracts.Communication.Messages;
using Members.Application;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using TGF.Common.ROP.HttpResult;
using TGF.CA.Application.Contracts.Communication;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    internal class MemberStatusUpdateHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberBanUpdated>
    {
        public async Task Handle(IntegrationMessage<MemberBanUpdated> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var lMembersService = lScope.ServiceProvider.GetRequiredService<IMembersService>();
            var lMemberStatus = aIntegrationMessage.Content.IsMemberBanned ? MemberStatusEnum.Banned : MemberStatusEnum.Active;

            await lMembersService.UpdateMemberStatus(ulong.Parse(aIntegrationMessage.Content.UserId), ulong.Parse(aIntegrationMessage.Content.GuildId), lMemberStatus, aCancellationToken)
            .Tap(member => lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>()
                      .Publish(new MemberTokenRevoked([member.Id]), routingKey: RoutingKeys.Members.Member_revoke));
        }
    }
}
