using Common.Application.DTOs.Members;
using Common.Infrastructure.Communication.Messages;
using Members.Application.UseCases.Members;
using Microsoft.Extensions.DependencyInjection;
using SwarmBot.Infrastructure.Communication;
using TGF.CA.Infrastructure.Communication.Consumer.Handler;
using TGF.CA.Infrastructure.Communication.Messages;
using TGF.CA.Infrastructure.Communication.Messages.Discord;
using TGF.CA.Infrastructure.Communication.Publisher.Integration;
using TGF.Common.ROP.HttpResult;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    internal class MemberRoleAssignedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberRoleAssigned>
    {
        public async Task Handle(IntegrationMessage<MemberRoleAssigned> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var assignMemberRolesUseCase = lScope.ServiceProvider.GetRequiredService<AssignMemberRoles>();

            await assignMemberRolesUseCase.ExecuteAsync(new MemberRolesDTO(aIntegrationMessage.Content.UserId, aIntegrationMessage.Content.GuildId, aIntegrationMessage.Content.DiscordRoleList.Select(x => x.Id)), aCancellationToken)
            .Tap(updateMemberRolesResultDto =>
            {
                if (updateMemberRolesResultDto.IsPermissionsChanged)
                    lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>()
                        .Publish(new MemberTokenRevoked([updateMemberRolesResultDto.Member.Id]), routingKey: RoutingKeys.Members.Member_revoke);
            });
        }
    }
}
