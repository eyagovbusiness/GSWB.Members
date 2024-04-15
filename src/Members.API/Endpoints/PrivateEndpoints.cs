using Common.Application.DTOs.Members;
using Common.Infrastructure.Communication.ApiRoutes;
using Members.Application;
using Microsoft.AspNetCore.Mvc;
using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;

namespace Members.API.Endpoints
{
    /// List of private endpoint only reached from the internal private docker network.
    public class PrivateEndpoints : IEndpointDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MembersApiRoutes.private_members_getByDiscordUserId, Get_GetByDiscordUserId);
            aWebApplication.MapPut(MembersApiRoutes.private_members_addNew, Put_NewMember);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// private endpoint implementation 
        private async Task<IResult> Put_NewMember([FromBody] CreateMemberDTO aCreateMemberDTO, IMembersService aMembersService, CancellationToken aCancellationToken = default)
            => await aMembersService.AddNewMember(aCreateMemberDTO, aCancellationToken)
            .ToIResult();

        /// private endpoint implementation 
        private async Task<IResult> Get_GetByDiscordUserId(ulong aDiscordUserId, IMembersService aMembersService, CancellationToken aCancellationToken = default)
            => await aMembersService.GetByDiscordUserId(aDiscordUserId, aCancellationToken)
            .ToIResult();

    }
}
