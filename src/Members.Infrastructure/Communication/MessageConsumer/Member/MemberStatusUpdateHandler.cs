using Common.Application.Contracts.Communication;
using Common.Domain.ValueObjects;
using Common.Application.Contracts.Communication.Messages;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.CA.Application.Contracts.Communication;
using Members.Application.UseCases.Members.Update;
using Members.Domain.ValueObjects;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    internal class MemberStatusUpdateHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberBanUpdated>
    {
        public async Task Handle(IntegrationMessage<MemberBanUpdated> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var updateMemberStatusUseCase = lScope.ServiceProvider.GetRequiredService<UpdateMemberStatus>();
            var memberStatus = aIntegrationMessage.Content.IsMemberBanned ? MemberStatusEnum.Banned : MemberStatusEnum.Active;

            _ = await updateMemberStatusUseCase.ExecuteAsync(
                new MemberStatus(
                    new MemberKey(ulong.Parse(aIntegrationMessage.Content.GuildId), ulong.Parse(aIntegrationMessage.Content.UserId)),
                    memberStatus
                )
            , aCancellationToken)
            .Tap(member => lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>()
                .Publish(new MemberTokenRevoked([new MemberKey(member.GuildId, member.UserId)]), routingKey: RoutingKeys.Members.Member_revoke));
        }
    }
}
