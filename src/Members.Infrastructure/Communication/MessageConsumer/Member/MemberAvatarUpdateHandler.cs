using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Comm.Consumer.Handler;
using TGF.CA.Infrastructure.Comm.Messages;
using Common.Application.Contracts.Communication.Messages.Discord;
using Members.Application.UseCases.Members.Update;
using Common.Domain.ValueObjects;
using Members.Domain.ValueObjects;

namespace Members.Infrastructure.Communication.MessageConsumer.Member
{
    internal class MemberAvatarUpdateHandler(IServiceScopeFactory aServiceScopeFactory)
        : IIntegrationMessageHandler<MemberAvatarUpdated>
    {
        public async Task Handle(IntegrationMessage<MemberAvatarUpdated> aIntegrationMessage, CancellationToken aCancellationToken = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var updateMemberAvatarUseCase = lScope.ServiceProvider.GetRequiredService<UpdateMemberAvatar>();
            _ = await updateMemberAvatarUseCase.ExecuteAsync(
                new MemberAvatar(
                    new MemberKey(ulong.Parse(aIntegrationMessage.Content.GuildId), ulong.Parse(aIntegrationMessage.Content.UserId)),
                    aIntegrationMessage.Content.NewAvatarUrl
                )
            , aCancellationToken);
        }
    }
}
