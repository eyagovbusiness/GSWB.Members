using Common.Application.Contracts.Communication.Messages;
using Common.Application.Contracts.Communication;
using Common.Domain.ValueObjects;
using Members.Domain.Contracts.Repositories;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.CA.Application.Contracts.Communication;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use Case to delete a member by its Id.
    /// </summary>
    public class DeleteMember(
        IMemberRepository memberRepository,
        IIntegrationMessagePublisher integrationMessagePublisher
    )
         : IUseCase<IHttpResult<Unit>, MemberKey>
    {
        public async Task<IHttpResult<Unit>> ExecuteAsync(MemberKey request, CancellationToken aCancellationToken = default)
        => await memberRepository.GetByIdAsync(request, aCancellationToken)
        .Bind(member => memberRepository.DeleteAsync(member!, aCancellationToken))
        .Tap(updatedRoleIdList => integrationMessagePublisher.Publish(new MemberTokenRevoked([request]), routingKey: RoutingKeys.Members.Member_revoke))
        .Map(_ => Unit.Value);
    }
}
