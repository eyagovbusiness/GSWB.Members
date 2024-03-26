using Common.Application.DTOs;
using Common.Infrastructure.Communication.ApiRoutes;
using Common.Infrastructure.Communication.Messages;
using Members.Application;
using Microsoft.AspNetCore.Mvc;
using SwarmBot.Infrastructure.Communication;
using System.Security.Claims;
using TGF.CA.Infrastructure.Communication.Publisher.Integration;
using TGF.CA.Infrastructure.Security.Identity.Authentication;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;

namespace Members.API.Endpoints
{
    /// Collection of endpoints to run over the authenticated member(by JWT).
    public class MeEndpoints : IEndpointDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MembersApiRoutes.members_me, Get_Me).RequireJWTBearer().SetResponseMetadata<MemberDetailDTO>(200, 404);
            aWebApplication.MapPut(MembersApiRoutes.members_me_update, Put_MeUpdate).RequireJWTBearer().SetResponseMetadata<MemberDetailDTO>(200, 404);
            aWebApplication.MapDelete(MembersApiRoutes.members_me_delete, Delete_MeDelete).RequireJWTBearer().SetResponseMetadata<Unit>(200, 404);
            aWebApplication.MapGet(MembersApiRoutes.members_me_getVerifyInfo, Get_GetVerifyInfo).RequireJWTBearer().SetResponseMetadata<MemberVerifyInfoDTO>(200, 404);
            aWebApplication.MapPut(MembersApiRoutes.members_me_verifyGameHandle, Put_MeVerifyGameHandle).RequireJWTBearer().SetResponseMetadata<MemberDetailDTO>(200, 404);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }


        /// <summary>
        /// Get the current authenticated member's overall information(<see cref="MemberDetailDTO"/>).
        /// </summary>
        private async Task<IResult> Get_Me(IMembersService aMembersService, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await aMembersService.GetDetailByDiscordUserId(ulong.Parse(aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Updates the basic profile data(<see cref="MemberProfileUpdateDTO"/>) for the current authenticated member.
        /// </summary>
        private async Task<IResult> Put_MeUpdate([FromBody] MemberProfileUpdateDTO aMemberProfileDTO, IMembersService aMembersService, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await aMembersService.UpdateMemberDetail(aMemberProfileDTO, ulong.Parse(aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Delete the current authenticated member from database.
        /// </summary>
        private async Task<IResult> Delete_MeDelete(IMembersService aMembersService, ClaimsPrincipal aClaims, IIntegrationMessagePublisher aIntegrationPublisherService, CancellationToken aCancellationToken = default)
            => await aMembersService.DeleteMember(ulong.Parse(aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
            .Tap(updatedRoleIdList => aIntegrationPublisherService.Publish(new MemberTokenRevoked([aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!]), routingKey: RoutingKeys.Members.Member_revoke))
            .Map(_ => Unit.Value)
            .ToIResult();

        #region Verify

        /// <summary>
        /// Get game handle verification info for the authenticated member. Calling this endpoint refreshes the verification code if expired.
        /// </summary>
        private async Task<IResult> Get_GetVerifyInfo(IMembersService aMembersService, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await aMembersService.Get_GetVerifyInfo(ulong.Parse(aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Verifies the GameHandle ownership of current authenticated member by checking if the public profile of the provided GameHandle by this member contains this member's personal verification code. Returns the updated member resulting after the verification process(<see cref="MemberDetailDTO"/>).
        /// </summary>
        private async Task<IResult> Put_MeVerifyGameHandle(IMembersService aMembersService, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await aMembersService.VerifyGameHandle(ulong.Parse(aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
            .ToIResult();

        #endregion


    }
}
