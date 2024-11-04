using Common.Application.DTOs.Members;
using Common.Domain.Validation;
using Members.Application;
using Microsoft.AspNetCore.Mvc;
using TGF.CA.Presentation;
using TGF.Common.ROP.Result;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using Members.Application.UseCases.Members;
using Common.Application.Communication.Routing;
using Common.Domain.ValueObjects;
using TGF.CA.Presentation.MinimalAPI;

namespace Members.API.Endpoints
{
    /// List of private endpoint only reached from the internal private docker network.
    public class PrivateEndpoints : IEndpointsDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MembersApiRoutes.private_members_id, Get_GetMemberById);
            aWebApplication.MapGet(MembersApiRoutes.private_members_userId_guildId, Get_MemberByUserAndGuildIds);
            aWebApplication.MapGet(MembersApiRoutes.private_members_permissions, Get_Permissions);
            aWebApplication.MapPut(MembersApiRoutes.private_members, Put_NewMember);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// private endpoint implementation 
        private async Task<IResult> Put_NewMember([FromBody] CreateMemberDTO aCreateMemberDTO, DiscordIdValidator discordIdValidator, AddMember addMember, CancellationToken aCancellationToken = default)
        => await Result.ValidationResult(discordIdValidator.Validate(aCreateMemberDTO.GuildId))
        .Bind(_ => addMember.ExecuteAsync(aCreateMemberDTO, aCancellationToken))
        .ToIResult();

        /// private endpoint implementation 
        private async Task<IResult> Get_Permissions(Guid id, GetMemberPermissions getMemberPermissions, CancellationToken aCancellationToken = default)
        => await getMemberPermissions.ExecuteAsync(id, aCancellationToken)
        .ToIResult(); 

        /// private endpoint implementation 
        private async Task<IResult> Get_GetMemberById(Guid id, IMembersService aMembersService, CancellationToken aCancellationToken = default)
        => await aMembersService.GetByDiscordUserId(id, aCancellationToken)
        .ToIResult();

        /// private endpoint implementation 
        private async Task<IResult> Get_MemberByUserAndGuildIds(string userId, string guildId, DiscordIdValidator discordIdValidator, GetMemberDetail getMemberDetail, CancellationToken aCancellationToken = default)
        => await Result.ValidationResult(discordIdValidator.Validate(userId))
        .Validate(userId, discordIdValidator)
        .Bind(_ => getMemberDetail.ExecuteAsync(new GuildAndUserId(ulong.Parse(guildId),ulong.Parse(userId)), aCancellationToken))
        .ToIResult();

    }
}
