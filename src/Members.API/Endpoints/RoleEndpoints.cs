using Common.Application.Communication.Routing;
using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.Validation;
using Common.Domain.ValueObjects;
using Members.Application.Specifications;
using Members.Application.UseCases.Guilds.Roles;
using Members.Application.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.CA.Application.Validation;
using TGF.CA.Infrastructure.Identity.Authorization.Permissions;
using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.Result;
using System.Security.Claims;
using Common.Infrastructure.Security;

namespace Members.API.Endpoints
{
    /// List of endpoints related with the guild's roles.
    public class RoleEndpoints : IEndpointsDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MembersApiRoutes.guilds_mine_roles, Get_Roles).RequirePermissions(PermissionsEnum.Admin).SetResponseMetadata<PaginatedRoleListDTO>(200);
            aWebApplication.MapPut(MembersApiRoutes.guilds_mine_roles, Put_UpdateRoleList).RequirePermissions(PermissionsEnum.Admin).SetResponseMetadata<RoleDTO[]>(200, 404);

        }
        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// <summary>
        /// Get the list of guild members(<see cref="PaginatedRoleListDTO"/>) under filtering and pagination conditions specified in the request's query parameters and sorted by a given column name.
        /// </summary>
        private async Task<IResult> Get_Roles(ClaimsPrincipal aClaims, ListGuildRoles listGuildRoles, PaginationValidator paginationValidationRules, RoleSortingValidator sortingValidationRules, DiscordIdValidator discordIdValidator,
        int? page, int? pageSize,
        string? sortBy, ListSortDirection? sortDirection,
        string? name,
        CancellationToken aCancellationToken = default)
        => await new GuildRolesPageSpecification(
            aClaims.FindFirstValue(GuildSwarmClaims.GuildId)!,
            page, pageSize, sortBy, sortDirection,
            paginationValidationRules, sortingValidationRules, discordIdValidator,
            name
        ).Apply()
        .Bind(specification => listGuildRoles.ExecuteAsync(specification, aCancellationToken))
        .ToIResult();

        /// <summary>
        /// Updates a list of application roles: ONLY Permissions, RoleType and Description columns can be updated. Ssee RoleUpdateDTO.DiscordRoleId=> is used only to determine which role has to be updated. To change the name or position it must be changed on Discord. 
        /// </summary>
        private async Task<IResult> Put_UpdateRoleList(ClaimsPrincipal aClaims, [FromBody] IEnumerable<RoleUpdateDTO> aRoleDTOList, UpdateRoles updateRoles, CancellationToken aCancellationToken = default)
        => await updateRoles.ExecuteAsync(new GuildRolesUpdateDTO(aClaims.FindFirstValue(GuildSwarmClaims.GuildId)!, aRoleDTOList), aCancellationToken)
        .ToIResult();
    }
}
