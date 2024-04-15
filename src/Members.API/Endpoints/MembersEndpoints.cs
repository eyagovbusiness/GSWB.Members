using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Common.Infrastructure.Communication.ApiRoutes;
using Common.Presentation.Validation;
using Members.API.Validation;
using Members.Application;
using TGF.CA.Infrastructure.Security.Identity.Authorization.Permissions;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.API.Endpoints
{
    /// Collection of endpoints to run over the whole guild's member list.
    public class MembersEndpoints : IEndpointDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MembersApiRoutes.members_list, Get_MembersList)
                .RequirePermissions(PermissionsEnum.AccessMembers)
                .SetResponseMetadata<PaginatedMemberListDTO[]>(200)
                .ProducesValidationProblem();

            aWebApplication.MapGet(MembersApiRoutes.members_count, Get_MembersCount)
                .SetResponseMetadata<int>(200);
        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// <summary>
        /// Get the list of guild members(<see cref="PaginatedMemberListDTO"/>) under filtering and pagination conditions specified in the request's query parameters and sorted by a given column name.
        /// </summary>
        private async Task<IResult> Get_MembersList(IMembersService aMembersService, PaginationValidator aPaginationValidator, MembersSortByValidator aSortByValidator,
            string? discordNameFilter, string? gameHandleFilter, ulong? roleIdFilter, bool? isVerifiedFilter,
            int page = 1, int pageSize = 20, string sortBy = nameof(MemberDTO.Roles),
            CancellationToken aCancellationToken = default)
        =>
        await Result.CancellationTokenResult(aCancellationToken)
        .ValidateMany(
            aPaginationValidator.Validate(new PaginationValParams(page, pageSize)),
            aSortByValidator.Validate(sortBy))
        .Bind(_ => aMembersService.GetMemberList(page, pageSize, sortBy, discordNameFilter, gameHandleFilter, roleIdFilter, isVerifiedFilter, aCancellationToken))
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
