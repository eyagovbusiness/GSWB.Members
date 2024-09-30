using Common.Application.DTOs.Guilds;
using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Common.Infrastructure.Communication.ApiRoutes;
using Common.Infrastructure.Security;
using Common.Presentation.Validation;
using HealthChecks.UI.Client;
using Members.API.Validation;
using Members.Application;
using Members.Application.UseCases.Guilds;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TGF.CA.Infrastructure.Security.Identity.Authorization.Permissions;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.API.Endpoints
{
    /// Collection of endpoints to run over the guilds
    public class GuildEndpoints : IEndpointDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            //aWebApplication.MapGet(MembersApiRoutes.members_guilds, Get_MemberGuildList)
            //    .SetResponseMetadata<MemberGuildDTO[]>(200);

            aWebApplication.MapGet(MembersApiRoutes.members_me_guild, Get_MemberGuild)
                .SetResponseMetadata<GuildDTO>(200);

            //aWebApplication.MapPut(MembersApiRoutes.members_guild, Get_MembersList)
            //    .RequirePermissions(PermissionsEnum.Admin)
            //    .SetResponseMetadata<GuildDTO>(200);
        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        ///// <summary>
        ///// Get the list of guild members(<see cref="PaginatedMemberListDTO"/>) under filtering and pagination conditions specified in the request's query parameters and sorted by a given column name.
        ///// </summary>
        //private async Task<IResult> Get_MemberGuildList(IMembersService aMembersService, PaginationValidator aPaginationValidator, MembersSortByValidator aSortByValidator,CancellationToken aCancellationToken = default)
        //=>
        //await Result.CancellationTokenResult(aCancellationToken)
        //.ValidateMany(
        //    aPaginationValidator.Validate(new PaginationValParams(page, pageSize)),
        //    aSortByValidator.Validate(sortBy))
        //.Bind(_ => aMembersService.GetMemberList(page, pageSize, sortBy, discordNameFilter, gameHandleFilter, roleIdFilter, isVerifiedFilter, aCancellationToken))
        //.ToIResult();

        private async Task<IResult> Get_MemberGuild(HttpContext httpContext, GetGuild getGuildUseCase, CancellationToken aCancellationToken = default)
            => await getGuildUseCase.ExecuteAsync(httpContext.User.Claims.First(c => c.Type == GuildSwarmClaims.GuildId).Value)
            .ToIResult();


        /// <summary>
        /// Get the list of guild members(<see cref="MemberDetailDTO"/>) from the provided members id list.
        /// </summary>
        private async Task<IResult> Post_MembersByIdList(IMembersService aMembersService, [FromBody] IEnumerable<Guid> aMemberList, CancellationToken aCancellationToken = default)
        =>
        await Result.CancellationTokenResult(aCancellationToken)
        .Bind(_ => aMembersService.GetMembersByIdList(aMemberList, aCancellationToken))
        .ToIResult();

        /// <summary>
        /// Get the count of all the guild's members registered in the database.
        /// </summary>
        private async Task<IResult> Get_MembersCount(IMembersService aMembersService, CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResult(aCancellationToken)
            .Bind(_ => aMembersService.GetMembersCount())
            .ToIResult();
    }
}
