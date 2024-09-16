using Common.Application.DTOs.Members;
using Common.Infrastructure.Communication.ApiRoutes;
using Members.Application;
using Members.Application.UseCases.Member;
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
            aWebApplication.MapGet(MembersApiRoutes.private_members_discordUserId, Get_GetByDiscordUserId);
            aWebApplication.MapGet(MembersApiRoutes.private_members_permissions, Get_Permissions);
            aWebApplication.MapPut(MembersApiRoutes.private_members, Put_NewMember);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// private endpoint implementation 
        private async Task<IResult> Put_NewMember([FromBody] CreateMemberDTO aCreateMemberDTO, IMembersService aMembersService, CancellationToken aCancellationToken = default)
            => await aMembersService.AddNewMember(aCreateMemberDTO, aCancellationToken)
            .ToIResult();

        private async Task<IResult> Get_Permissions(Guid id, GetMemberPermissionsUseCase aGetMemberPermissionsUseCase, CancellationToken aCancellationToken = default)
            => await aGetMemberPermissionsUseCase.ExecuteAsync(id, aCancellationToken)
            .ToIResult(); 

        /// private endpoint implementation 
        private async Task<IResult> Get_GetByDiscordUserId(ulong id, IMembersService aMembersService, CancellationToken aCancellationToken = default)
            => await aMembersService.GetByDiscordUserId(id, aCancellationToken)
            .ToIResult();

    }
}
