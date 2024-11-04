using Common.Application.Contracts.Communication;
using Common.Application.DTOs.Members;
using Common.Application.Contracts.Communication.Messages;
using Members.Application.UseCases.Members;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.CA.Application.Contracts.Communication;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    internal class MemberRoleAssignedHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberRoleAssigned>
    {
        public async Task Handle(IntegrationMessage<MemberRoleAssigned> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var assignMemberRolesUseCase = lScope.ServiceProvider.GetRequiredService<AssignMemberRoles>();

            await assignMemberRolesUseCase.ExecuteAsync(new MemberRolesDTO(aIntegrationMessage.Content.UserId, aIntegrationMessage.Content.GuildId, aIntegrationMessage.Content.DiscordRoleList.Select(x => x.RoleId)), aCancellationToken)
            .Tap(updateMemberRolesResultDto =>
            {
                if (updateMemberRolesResultDto.IsPermissionsChanged)
                    lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>()
                        .Publish(new MemberTokenRevoked([updateMemberRolesResultDto.Member.Id]), routingKey: RoutingKeys.Members.Member_revoke);
            });
        }
    }
}
