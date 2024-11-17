using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using Members.Application.UseCases.Members.Update;
using Members.Domain.ValueObjects;
using Common.Domain.ValueObjects;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    internal class MemberRenameHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberRenamed>
    {
        public async Task Handle(IntegrationMessage<MemberRenamed> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var updateMemberDiscordDisplayNameUseCase = lScope.ServiceProvider.GetRequiredService<UpdateMemberDiscordDisplayName>();
            _ = await updateMemberDiscordDisplayNameUseCase.ExecuteAsync(
                new MemberDiscordDisplayName(
                    new MemberKey(ulong.Parse(aIntegrationMessage.Content.GuildId), ulong.Parse(aIntegrationMessage.Content.UserId)), 
                    aIntegrationMessage.Content.NewDisplayName
                )
            , aCancellationToken);
        }
    }
}
