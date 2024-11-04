using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using Members.Application.UseCases.Guilds.Roles;

namespace Members.Infrastructure.Communication.MessageConsumer.Role
{
    internal class RoleUpdatedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<RoleUpdated>
    {
        public async Task Handle(IntegrationMessage<RoleUpdated> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var updateRolesUseCase = lScope.ServiceProvider.GetRequiredService<UpdateRoleDiscordData>();
            _ = await updateRolesUseCase.ExecuteAsync(aIntegrationMessage.Content.DiscordRole, aCancellationToken);
        }
    }
}
