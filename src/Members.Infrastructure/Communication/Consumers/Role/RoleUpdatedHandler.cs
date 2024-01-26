using Members.Application;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using TGF.CA.Infrastructure.Communication.Messages.Discord;

namespace Members.Infrastructure.Communication.MessageConsumer.Role
{
    internal class RoleUpdatedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<RoleUpdated>
    {
        public async Task Handle(IntegrationMessage<RoleUpdated> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var lRolesService = lScope.ServiceProvider.GetRequiredService<IRolesService>();
            _ = await lRolesService.UpdateRoleAsync(aIntegrationMessage.Content.DiscordRole, aCancellationToken);
        }
    }
}
