using Members.Application;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using TGF.CA.Infrastructure.Communication.Messages.Discord;

namespace Members.Infrastructure.MessageConsumer.Member
{
    internal class MemberAvatarUpdateHandler(IServiceScopeFactory aServiceScopeFactory) 
        : IIntegrationMessageHandler<MemberAvatarUpdated>
    {
        public async Task Handle(IntegrationMessage<MemberAvatarUpdated> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var lMembersService = lScope.ServiceProvider.GetRequiredService<IMembersService>();
            _ = await lMembersService.UpdateMemberAvatar(ulong.Parse(aIntegrationMessage.Content.DiscordUserId), aIntegrationMessage.Content.NewAvatarUrl, aCancellationToken);
        }
    }
}
