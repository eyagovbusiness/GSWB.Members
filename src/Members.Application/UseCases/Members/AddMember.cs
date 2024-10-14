using Common.Application.Contracts.Services;
using Common.Application.DTOs.Auth;
using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Members;
using Members.Application.Mapping;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

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
            var lExistingMemberResultResult = await memberRepository.GetByUserAndGuildIdsAsync(ulong.Parse(request.DiscordCookieUserInfo.UserNameIdentifier), ulong.Parse(request.GuildId), aCancellationToken);
            if (lExistingMemberResultResult.IsSuccess)
                return Result.Failure<MemberDetailDTO>(ApplicationErrors.Members.DiscordAccountAlreadyRegistered);

            var lDiscordRoleListResult = await Result.CancellationTokenResult(aCancellationToken)
            .Bind(_ => swarmBotCommunicationService.GetMemberRoleList(request.GuildId, request.DiscordCookieUserInfo.UserNameIdentifier, aCancellationToken));


            var lNewMemberResult = await lDiscordRoleListResult
            .Bind(_ => swarmBotCommunicationService.GetMemberProfileFromId(request.GuildId, request.DiscordCookieUserInfo.UserNameIdentifier))
            .Map(discordMemberProfile => this.GetNewMemberEntity(request, discordMemberProfile))
            .Bind(newMember => memberRepository.Add(newMember, aCancellationToken))
            .Map(newMember => newMember.ToDetailDto(aIncludeDiscordOnlyRoles: false));

            var assignRolesResult = lNewMemberResult
            .Bind(discordRoleList => assignMemberRoles.ExecuteAsync(new MemberRolesDTO(request.GuildId, request.DiscordCookieUserInfo.UserNameIdentifier, lDiscordRoleListResult.Value.Select(role => role.Id))));

            return lNewMemberResult;

        }

        private Member GetNewMemberEntity(CreateMemberDTO aCreateMemberDTO, DiscordProfileDTO aDiscordProfileDTO)
        => new(
            UserId: aCreateMemberDTO.DiscordCookieUserInfo.UserNameIdentifier,
            aCreateMemberDTO.GuildId,
            DiscordGuildDisplayName: GetDiscordGuildDisplayName(aCreateMemberDTO.DiscordCookieUserInfo, aDiscordProfileDTO),
            DiscordAvatarUrl: aDiscordProfileDTO.AvatarUrl,
            GameHandle: aCreateMemberDTO.SignUpData?.GameHandle,
            SpectrumCommunityMoniker: aCreateMemberDTO.SignUpData?.SpectrumCommunityMoniker);

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
