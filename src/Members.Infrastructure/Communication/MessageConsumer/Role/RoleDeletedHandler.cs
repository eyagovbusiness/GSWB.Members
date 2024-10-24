using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using Members.Application.UseCases.Guilds.Roles;

namespace Members.Infrastructure.Communication.MessageConsumer.Role
{
    internal class RoleDeletedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<RoleDeleted>
    {
        public async Task Handle(IntegrationMessage<RoleDeleted> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var deleteRoleUseCase = lScope.ServiceProvider.GetRequiredService<DeleteRole>();
            _ = await deleteRoleUseCase.ExecuteAsync(aIntegrationMessage.Content, aCancellationToken);
        }
    }
}
