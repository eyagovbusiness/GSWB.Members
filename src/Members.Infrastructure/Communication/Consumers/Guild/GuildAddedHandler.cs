using Common.Application.Guilds;
using Members.Application.UseCases.Guilds;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using TGF.CA.Infrastructure.Communication.Messages.Discord;

namespace Members.Infrastructure.Communication.MessageConsumer.Guild
{
    internal class GuildAddedHandler(IServiceScopeFactory serviceScopeFactory)
        : IIntegrationMessageHandler<GuildAdded>
    {
        public async Task Handle(IntegrationMessage<GuildAdded> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var addGuildUseCase = scope.ServiceProvider.GetRequiredService<AddGuildUseCase>();
            _ = await addGuildUseCase.ExecuteAsync(new GuildDTO(aIntegrationMessage.Content.GuildId, aIntegrationMessage.Content.GuildName, aIntegrationMessage.Content.IconUrl), aCancellationToken);
        }
    }
}
