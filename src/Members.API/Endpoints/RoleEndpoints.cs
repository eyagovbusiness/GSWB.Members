using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Common.Infrastructure.Communication.ApiRoutes;
using Common.Infrastructure.Communication.Messages;
using Common.Presentation.Validation;
using Members.API.Validation;
using Members.Application;
using Microsoft.AspNetCore.Mvc;
using SwarmBot.Infrastructure.Communication;
using TGF.CA.Infrastructure.Communication.Publisher.Integration;
using TGF.CA.Infrastructure.Security.Identity.Authorization.Permissions;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.API.Endpoints
{
    /// List of endpoints related with the guild's roles.
    public class RoleEndpoints : IEndpointDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapPost(MembersApiRoutes.roles_discordSync, Post_SyncRolesWithDiscord).RequirePermissions(PermissionsEnum.Admin).SetResponseMetadata(200).ProducesValidationProblem();
            aWebApplication.MapGet(MembersApiRoutes.roles, Get_Roles).RequirePermissions(PermissionsEnum.Admin).SetResponseMetadata<PaginatedRoleListDTO>(200);
            aWebApplication.MapPut(MembersApiRoutes.roles, Put_UpdateRoleList).RequirePermissions(PermissionsEnum.Admin).SetResponseMetadata<RoleDTO[]>(200, 404);

        }
        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// <summary>
        /// Synchronizes the application roles with the Discord guild's server roles.
        /// </summary>
        private async Task Post_SyncRolesWithDiscord(IRolesInfrastructureService aRolesInfrastructureService, CancellationToken aCancellationToken = default)
            => await aRolesInfrastructureService.SyncRolesWithDiscordAsync(aCancellationToken);

        /// <summary>
        /// Get the list of guild members(<see cref="PaginatedRoleListDTO"/>) under filtering and pagination conditions specified in the request's query parameters and sorted by a given column name.
        /// </summary>
        private async Task<IResult> Get_Roles(IRolesService aRolesService, PaginationValidator aPaginationValidator, RolesSortByValidator aSortByValidator,
            string? name, RoleTypesEnum? type, PermissionsEnum? permissions,
            int page = 1, int pageSize = 20, string sortBy = nameof(RoleDTO.Position),
            CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResult(aCancellationToken)
        .ValidateMany(
            aPaginationValidator.Validate(new PaginationValParams(page, pageSize)),
            aSortByValidator.Validate(sortBy))
        .Bind(_ => aRolesService.GetRoleList(page, pageSize, sortBy, name, type, permissions, aCancellationToken))
        .ToIResult();

        /// <summary>
        /// Updates a list of application roles: ONLY Permissions, RoleType and Description columns can be updated. (<see cref="RoleUpdateDTO.DiscordRoleId"/>) => is used only to determine which role has to be updated. To change the name or position it must be changed on Discord. 
        /// </summary>
        private async Task<IResult> Put_UpdateRoleList([FromBody] IEnumerable<RoleUpdateDTO> aRoleDTOList, IRolesService aRolesService, IIntegrationMessagePublisher aIntegrationPublisherService, CancellationToken aCancellationToken = default)
        => await aRolesService.UpdateRoleList(aRoleDTOList, aCancellationToken)
        .Tap(roleDTOList =>
        {
            var lUpdatedRoleIdList = roleDTOList.Select(roleDTO => ulong.Parse(roleDTO.Id));
            aIntegrationPublisherService.Publish(new RoleTokenRevoked(lUpdatedRoleIdList.ToArray()), routingKey: RoutingKeys.Members.Member_role_revoke);
        })
        .ToIResult();
    }
}
