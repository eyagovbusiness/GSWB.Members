using Members.Application;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using TGF.CA.Infrastructure.Communication.Messages.Discord;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    internal class MemberRenameHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberRenamed>
    {
        public async Task Handle(IntegrationMessage<MemberRenamed> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var lMembersService = lScope.ServiceProvider.GetRequiredService<IMembersService>();
            _ = await lMembersService.UpdateMemberDiscordDisplayName(ulong.Parse(aIntegrationMessage.Content.UserId), ulong.Parse(aIntegrationMessage.Content.GuildId), aIntegrationMessage.Content.NewDisplayName, aCancellationToken);
        }
    }
}
