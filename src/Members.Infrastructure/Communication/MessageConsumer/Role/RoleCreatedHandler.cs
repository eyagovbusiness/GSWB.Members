using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using Members.Application.UseCases.Guilds.Roles;

namespace Members.Infrastructure.Communication.MessageConsumer.Role
{
    internal class GuildAddedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<RoleCreated>
    {
        public async Task Handle(IntegrationMessage<RoleCreated> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var createRolesUseCase = lScope.ServiceProvider.GetRequiredService<CreateRoles>();
            _ = await createRolesUseCase.ExecuteAsync(aIntegrationMessage.Content, aCancellationToken);
        }
    }
}
