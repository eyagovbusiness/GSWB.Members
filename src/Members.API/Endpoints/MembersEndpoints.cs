using Common.Application.Communication.Routing;
using Common.Application.DTOs.Members;
using Common.Domain.Validation;
using Common.Domain.ValueObjects;
using Common.Infrastructure.Security;
using Members.Application;
using Members.Application.Specifications;
using Members.Application.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using TGF.CA.Application.DTOs;
using TGF.CA.Application.Validation;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.CA.Infrastructure.Identity.Authentication;
using TGF.CA.Infrastructure.Identity.Authorization.Permissions;
using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.Result;
using TGF.Common.ROP;
using Members.Application.UseCases.Members.Read;

namespace Members.API.Endpoints
{
    /// Collection of endpoints to run over the whole guild's member list.
    public class MembersEndpoints : IEndpointsDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MembersApiRoutes.guilds_mine_members, Get_MembersPagedList)
                .RequireJWTBearer()
                .RequirePermissions(PermissionsEnum.AccessMembers)
                .SetResponseMetadata<PagedListDTO<MemberDTO>>(200)
                .ProducesValidationProblem();

            aWebApplication.MapPost(MembersApiRoutes.guilds_mine_members_getByIds, Post_MembersByIdList)
                .RequireJWTBearer()
                .RequirePermissions(PermissionsEnum.AccessMembers)
                .SetResponseMetadata<MemberDetailDTO[]>(200)
                .ProducesValidationProblem();

            aWebApplication.MapGet(MembersApiRoutes.guilds_mine_members_count, Get_MembersCount)
                .SetResponseMetadata<int>(200);
        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// <summary>
        /// Get the list of guild members under filtering and pagination conditions specified in the request's query parameters and sorted by a given column name.
        /// </summary>
        private async Task<IResult> Get_MembersPagedList(ClaimsPrincipal claimsPrincipal, GetMembersPage getMembersPageUseCase, PaginationValidator paginationValidationRules, MembersSortingValidator sortingValidationRules, DiscordIdValidator discordIdValidator,
        int? page, int? pageSize,
        string? sortBy, ListSortDirection? sortDirection,
        string? name,
        CancellationToken aCancellationToken = default)
        => await new MemberPageSpecification(
            page, pageSize, sortBy, sortDirection,
            paginationValidationRules, sortingValidationRules, discordIdValidator,
            claimsPrincipal.FindFirstValue(GuildSwarmClaims.GuildId)!, name
        ).Apply()
        .Bind(specification => getMembersPageUseCase.ExecuteAsync((specification as MemberPageSpecification)!, aCancellationToken))
        .ToIResult();


        /// <summary>
        /// Get the list of guild members(<see cref="MemberDetailDTO"/>) from the provided members id list.
        /// </summary>
        private async Task<IResult> Post_MembersByIdList(ListMembersByIds listMembersByIdsUseCase, [FromBody] IEnumerable<MemberKey> memberIdList, CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResult(aCancellationToken)
        .Bind(_ => listMembersByIdsUseCase.ExecuteAsync(memberIdList, aCancellationToken))
        .ToIResult();

        /// <summary>
        /// Get the count of all the guild's members registered in the database.
        /// </summary>
        private async Task<IResult> Get_MembersCount(GetMembersCount getMembersCountUseCase, CancellationToken cancellationToken = default)
        => await Result.CancellationTokenResult(cancellationToken)
            .Bind(_ => getMembersCountUseCase.ExecuteAsync(Unit.Value, cancellationToken))
            .ToIResult();
    }
}
