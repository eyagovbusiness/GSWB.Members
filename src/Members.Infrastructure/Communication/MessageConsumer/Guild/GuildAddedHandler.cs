using Common.Application.Contracts.Communication.Messages.Discord;
using Common.Application.DTOs.Guilds;
using Members.Application.UseCases.Guilds;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;

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
