using Members.Application;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using TGF.CA.Infrastructure.Communication.Messages.Discord;

namespace Members.Infrastructure.MessageConsumer.Role
{
    internal class RoleDeletedHandler(IServiceScopeFactory aServiceScopeFactory) 
        : IIntegrationMessageHandler<RoleDeleted>
    {
        public async Task Handle(IntegrationMessage<RoleDeleted> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var lRolesService = lScope.ServiceProvider.GetRequiredService<IRolesService>();
            _ = await lRolesService.DeleteRoleAsync(ulong.Parse(aIntegrationMessage.Content.DiscordRoleId), aCancellationToken);
        }
    }
}
