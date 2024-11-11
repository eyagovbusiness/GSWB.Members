//using Common.Application.Contracts.Communication.Messages;
//using Common.Application.Contracts.Communication;
//using Common.Application.DTOs.Members;
//using Common.Domain.ValueObjects;
//using Members.Domain.Contracts.Repositories;
//using Members.Domain.Entities;
//using TGF.CA.Application.UseCases;
//using TGF.Common.ROP;
//using TGF.Common.ROP.HttpResult;
//using TGF.Common.ROP.HttpResult.RailwaySwitches;
//using TGF.CA.Application.Contracts.Communication;

//namespace Members.Application.UseCases.Members.Me
//{
//    /// <summary>
//    /// Get the member me(the member associated to the currently logged user).
//    /// </summary>
//    public class DeleteMemberMe(
//        IMemberRepository memberRepository,
//        IIntegrationMessagePublisher integrationMessagePublisher
//    )
//        : IUseCase<IHttpResult<Unit>, MemberKey>
//    {
//        public async Task<IHttpResult<Unit>> ExecuteAsync(MemberKey request, CancellationToken cancellationToken = default)
//        {
//            Member lMember = default!;
//            return await memberRepository.GetByGuildAndUserIdsAsync(request.GuildId, request.UserId, cancellationToken)
//            .Tap(member => lMember = member)
//            .Bind(member => memberRepository.Delete(member!, cancellationToken))
//            .Tap(updatedRoleIdList => integrationMessagePublisher.Publish(new MemberTokenRevoked([Guid.Parse(aClaims.FindFirstValue(GuildSwarmClaims.MemberId)!)]/*replace by compsite key*/ ), routingKey: RoutingKeys.Members.Member_revoke))
//            .Map(_ => Unit.Value);
//        }
//    }
//}
