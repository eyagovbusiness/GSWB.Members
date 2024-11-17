using Common.Application.Contracts.Communication;
using Common.Application.DTOs.Guilds;
using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Common.Application.Contracts.Communication.Messages;
using Common.Infrastructure.Security;
using Members.Application;
using Members.Application.UseCases.Guilds;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.CA.Application.Contracts.Communication;
using Common.Application.Communication.Routing;
using Members.Application.UseCases.Members.Me;
using TGF.CA.Infrastructure.Identity.Authentication;
using TGF.CA.Infrastructure.Identity.Authorization.Permissions;
using Members.Application.UseCases.Members.Update;
using Members.Domain.ValueObjects;

namespace Members.API.Endpoints
{
    /// Collection of endpoints to run over the authenticated member(by JWT).
    public class MeEndpoints : IEndpointsDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MembersApiRoutes.members_me, Get_Me)
                .RequireJWTBearer()
                .SetResponseMetadata<MemberDetailDTO>(200, 404);

            aWebApplication.MapGet(MembersApiRoutes.members_me_guild, Get_MemberGuild)
                .RequireJWTBearer()
                .SetResponseMetadata<GuildDTO>(200);

            aWebApplication.MapDelete(MembersApiRoutes.members_me_guild, Delete_MemberGuild)
                .RequireJWTBearer()
                .RequirePermissions(PermissionsEnum.AccessMembers)
                .SetResponseMetadata<GuildDTO>(200);

            aWebApplication.MapPut(MembersApiRoutes.members_me, Put_MeUpdate)
                .RequireJWTBearer()
                .SetResponseMetadata<MemberDetailDTO>(200, 404);

            aWebApplication.MapDelete(MembersApiRoutes.members_me, Delete_MeDelete)
                .RequireJWTBearer()
                .SetResponseMetadata<Unit>(200, 404);

            aWebApplication.MapGet(MembersApiRoutes.members_me_verify, Get_GetVerifyInfo)
                .RequireJWTBearer()
                .SetResponseMetadata<MemberVerificationStateDTO>(200, 404);

            aWebApplication.MapPut(MembersApiRoutes.members_me_verify, Put_MeVerifyGameHandle)
                .RequireJWTBearer()
                .SetResponseMetadata<MemberDetailDTO>(200, 404);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }


        /// <summary>
        /// Get the current authenticated member's overall information(<see cref="MemberDetailDTO"/>).
        /// </summary>
        private async Task<IResult> Get_Me(GetMemberMe getMemberMe, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await getMemberMe.ExecuteAsync(new MemberKey(ulong.Parse(aClaims.FindFirstValue(GuildSwarmClaims.GuildId)!), ulong.Parse(aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!)), aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Get the Guild information of the current membership checked-in
        /// </summary>
        private async Task<IResult> Get_MemberGuild(HttpContext httpContext, GetGuild getGuildUseCase, CancellationToken aCancellationToken = default)
        => await getGuildUseCase.ExecuteAsync(httpContext.User.Claims.First(c => c.Type == GuildSwarmClaims.GuildId).Value)
        .ToIResult();

        /// <summary>
        /// Get the Guild information of the current membership checked-in
        /// </summary>
        private async Task<IResult> Delete_MemberGuild(HttpContext httpContext, DeleteGuild deleteGuild, CancellationToken aCancellationToken = default)
        => await deleteGuild.ExecuteAsync(httpContext.User.Claims.First(c => c.Type == GuildSwarmClaims.GuildId).Value)
        .ToIResult();

        /// <summary>
        /// Updates the basic profile data(<see cref="MemberGameProfileUpdateDTO"/>) for the current authenticated member.
        /// </summary>
        private async Task<IResult> Put_MeUpdate([FromBody] MemberGameProfileUpdateDTO memberGameProfileUpdateDTO, UpdateMemberGameProfile updateMemberGameProfileUseCase, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await updateMemberGameProfileUseCase.ExecuteAsync(
                new MemberGameProfileUpdate(
                    new MemberKey(aClaims.FindFirstValue(GuildSwarmClaims.GuildId)!, aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    memberGameProfileUpdateDTO.GameHandle,
                    memberGameProfileUpdateDTO.SpectrumCommunityMoniker), aCancellationToken
                )
            .ToIResult();

        /// <summary>
        /// Delete the current authenticated member from database.
        /// </summary>
        private async Task<IResult> Delete_MeDelete(IMembersService aMembersService, ClaimsPrincipal aClaims, IIntegrationMessagePublisher aIntegrationPublisherService, CancellationToken aCancellationToken = default)
        {
            var memberId = new MemberKey(aClaims.FindFirstValue(GuildSwarmClaims.GuildId)!, aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await aMembersService.DeleteMember(memberId, aCancellationToken)
            .Tap(updatedRoleIdList => aIntegrationPublisherService.Publish(new MemberTokenRevoked([memberId]), routingKey: RoutingKeys.Members.Member_revoke))
            .Map(_ => Unit.Value)
            .ToIResult();
        }

        #region Verify

        /// <summary>
        /// Get game handle verification info for the authenticated member. Calling this endpoint refreshes the verification code if expired.
        /// </summary>
        private async Task<IResult> Get_GetVerifyInfo(IMembersService aMembersService, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await aMembersService.Get_GetVerifyInfo(new MemberKey(aClaims.FindFirstValue(GuildSwarmClaims.GuildId)!, aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Verifies the GameHandle ownership of current authenticated member by checking if the public profile of the provided GameHandle by this member contains this member's personal verification code. Returns the updated member resulting after the verification process(<see cref="MemberDetailDTO"/>).
        /// </summary>
        private async Task<IResult> Put_MeVerifyGameHandle(IMembersService aMembersService, ClaimsPrincipal aClaims, CancellationToken aCancellationToken = default)
            => await aMembersService.VerifyGameHandle(new MemberKey(aClaims.FindFirstValue(GuildSwarmClaims.GuildId)!, aClaims.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
            .ToIResult();

        #endregion


    }
}
