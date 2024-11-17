using Common.Application.Contracts.Services;
using Common.Application.DTOs.Auth;
using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Members;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;
using Common.Domain.ValueObjects;
using Members.Application.UseCases.Members.Update;

namespace Members.Application.UseCases.Members
{
    public class AddMember(
        IMemberRepository memberRepository,
        ISwarmBotCommunicationService swarmBotCommunicationService,
        AssignMemberRoles assignMemberRoles
    )
         : IUseCase<IHttpResult<MemberDetailDTO>, CreateMemberDTO>
    {
        public async Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(CreateMemberDTO request, CancellationToken aCancellationToken = default)
        {
            var lExistingMemberResultResult = await memberRepository.GetByIdAsync(new MemberKey(request.GuildId,request.DiscordCookieUserInfo.UserNameIdentifier), aCancellationToken);
            if (lExistingMemberResultResult.IsSuccess)
                return Result.Failure<MemberDetailDTO>(ApplicationErrors.Members.DiscordAccountAlreadyRegistered);

            var lDiscordRoleListResult = await Result.CancellationTokenResult(aCancellationToken)
            .Bind(_ => swarmBotCommunicationService.GetMemberRoleList(request.GuildId, request.DiscordCookieUserInfo.UserNameIdentifier, aCancellationToken));


            return await lDiscordRoleListResult
            .Bind(_ => swarmBotCommunicationService.GetMemberProfileFromId(request.GuildId, request.DiscordCookieUserInfo.UserNameIdentifier))
            .Map(discordMemberProfile => this.GetNewMemberEntity(request, discordMemberProfile))
            .Bind(newMember => memberRepository.AddAsync(newMember, aCancellationToken))
            .Bind(member => assignMemberRoles.ExecuteAsync(new MemberRolesDTO(member.GuildId.ToString(), member.UserId.ToString(), lDiscordRoleListResult.Value.Select(role => role.RoleId))))
            .Map(updateResult => updateResult.Member);

        }

        private Member GetNewMemberEntity(CreateMemberDTO aCreateMemberDTO, DiscordProfileDTO aDiscordProfileDTO)
        => new(
            userId: aCreateMemberDTO.DiscordCookieUserInfo.UserNameIdentifier,
            guildId: aCreateMemberDTO.GuildId,
            discordGuildDisplayName: GetDiscordGuildDisplayName(aCreateMemberDTO.DiscordCookieUserInfo, aDiscordProfileDTO),
            discordAvatarUrl: aDiscordProfileDTO.AvatarUrl,
            gameHandle: aCreateMemberDTO.SignUpData?.GameHandle,
            spectrumCommunityMoniker: aCreateMemberDTO.SignUpData?.SpectrumCommunityMoniker);

        //TO-DO: GSWB-27
        private string GetDiscordGuildDisplayName(DiscordCookieUserInfo aDiscordCookieUserInfo, DiscordProfileDTO aDiscordProfileDTO)
        {
            if (string.IsNullOrWhiteSpace(aDiscordProfileDTO.UserDisplayName) && !string.IsNullOrWhiteSpace(aDiscordCookieUserInfo.GivenName))
                return aDiscordCookieUserInfo.GivenName;

            return !string.IsNullOrWhiteSpace(aDiscordProfileDTO.UserDisplayName)
                   ? aDiscordProfileDTO.UserDisplayName
                   : aDiscordCookieUserInfo.UserName;
        }



    }

}
