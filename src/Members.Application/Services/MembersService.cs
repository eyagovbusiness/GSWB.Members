using Common.Application.Contracts.Services;
using Common.Application.DTOs.Auth;
using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Contracts.Services;
using Members.Domain.Entities;
using System.Collections.Immutable;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Application.Services
{
    public class MembersService(
        IMemberRepository aMemberRepository,
        IRoleRepository aRoleRepository,
        IMemberRoleService aMemberRoleService,
        ISwarmBotCommunicationService aSwarmBotCommunicationService,
        IGameVerificationService aGameVerificationService) : IMembersService
    {
        private readonly IMemberRepository _memberRepository = aMemberRepository;
        private readonly IRoleRepository _roleRepository = aRoleRepository;
        private readonly IMemberRoleService _memberRoleService = aMemberRoleService;
        private readonly ISwarmBotCommunicationService _SwarmBotCommunicationService = aSwarmBotCommunicationService;
        private readonly IGameVerificationService _gameVerificationService = aGameVerificationService;

        #region IMembersService

        public async Task<IHttpResult<PaginatedMemberListDTO>> GetMemberList(
            int aPage, int aPageSize,
            string aSortBy,
            string? aDiscordNameFilter, string? aGameHandleFilter, ulong? aRoleIdFilter, bool? aIsVerifiedFilter,
            CancellationToken aCancellationToken = default)
        => await _memberRepository.GetMembersListAsync(aPage, aPageSize, aSortBy, aDiscordNameFilter, aGameHandleFilter, aRoleIdFilter, aIsVerifiedFilter, aCancellationToken)
            .Bind(memberList => GetPaginatedMemberListDTO(memberList, aPage, aPageSize));

        public async Task<IHttpResult<IEnumerable<MemberDetailDTO>>> GetMembersByIdList(IEnumerable<Guid> aMemberIdList, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByIdListAsync(aMemberIdList, aCancellationToken)
            .Map(memberList => memberList.Select(member => member.ToDetailDto()));


        public async Task<IHttpResult<int>> GetMembersCount(CancellationToken aCancellationToken = default)
        => await _memberRepository.GetCountAsync();


        public async Task<IHttpResult<MemberDetailDTO>> AddNewMember(CreateMemberDTO aCreateMemberDTO, CancellationToken aCancellationToken = default)
        {
            var lExistingMemberResultResult = await _memberRepository.GetByDiscordUserIdAsync(ulong.Parse(aCreateMemberDTO.DiscordCookieUserInfo.UserNameIdentifier), aCancellationToken);
            if (lExistingMemberResultResult.IsSuccess)
                return Result.Failure<MemberDetailDTO>(ApplicationErrors.Members.DiscordAccountAlreadyRegistered);

            var lApplicationRoleListResult = await Result.CancellationTokenResult(aCancellationToken)
                .Bind(_ => _SwarmBotCommunicationService.GetDiscordUserRoleList(aCreateMemberDTO.DiscordCookieUserInfo.UserNameIdentifier, aCancellationToken))
                .Bind(discordRoleList => _roleRepository.GetListByDiscordRoleId(discordRoleList.Select(r => ulong.Parse(r.Id)), aCancellationToken));

            var lNewMemberResult = await lApplicationRoleListResult
            .Bind(_ => _SwarmBotCommunicationService.GetMemberProfileFromId(aCreateMemberDTO.DiscordCookieUserInfo.UserNameIdentifier))
            .Map(discordMemberProfile => this.GetNewMemberEntity(aCreateMemberDTO, discordMemberProfile, lApplicationRoleListResult.Value))
            .Bind(newMember => _memberRepository.Add(newMember, aCancellationToken))
            .Map(newMember => newMember.ToDetailDto(aIncludeDiscordOnlyRoles: false));

            return lNewMemberResult;

        }

        public async Task<IHttpResult<MemberDTO>> GetByDiscordUserId(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Map(member => member!.ToDto());

        public async Task<IHttpResult<MemberDetailDTO>> GetDetailByDiscordUserId(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Map(member => member!.ToDetailDto());

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberDetail(MemberProfileUpdateDTO aMemberProfileDTO, ulong aDiscordUserId, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => UpdateMemberProfile(member!, aMemberProfileDTO, aCancellationToken))
            .Map(member => member.ToDetailDto());

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberDiscordDisplayName(ulong aDiscordUserId, string aNewDisplayName, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => UpdateMemberDisplayName(member!, aNewDisplayName, aCancellationToken))
            .Map(member => member.ToDetailDto());

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberAvatar(ulong aDiscordUserId, string aNewAvatarUrl, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => UpdateAvatar(member!, aNewAvatarUrl, aCancellationToken))
            .Map(member => member.ToDetailDto());

        public async Task<IHttpResult<bool>> AssignMemberRoleList(ulong aDiscordUserId, IEnumerable<DiscordRoleDTO> aAssignRoleList, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => AssignRoleList(member!, aAssignRoleList, aCancellationToken));

        public async Task<IHttpResult<bool>> RevokeMemberRoleList(ulong aDiscordUserId, IEnumerable<DiscordRoleDTO> aRevokeRoleList, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => RevokeRoleList(member!, aRevokeRoleList, aCancellationToken));

        public async Task<IHttpResult<Member>> DeleteMember(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
         => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => _memberRepository.Delete(member!, aCancellationToken));

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberStatus(ulong aDiscordUserId, MemberStatusEnum aMemberStatus, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => UpdateMemberStatus(member!, aMemberStatus, aCancellationToken))
            .Map(member => member.ToDetailDto());

        #region Verify

        public async Task<IHttpResult<MemberVerificationStateDTO>> Get_GetVerifyInfo(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken)
            .Bind(member => TryUpdateGameHandleVerifyCode(member!, aCancellationToken))
            .Map(member => new MemberVerifyInfoDTO(member.IsGameHandleVerified, member!.GameHandleVerificationCode, member.VerificationCodeExpiryDate));

        public async Task<IHttpResult<MemberDetailDTO>> VerifyGameHandle(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
        {
            var lMemberResult = await _memberRepository.GetByDiscordUserIdAsync(aDiscordUserId, aCancellationToken);
            return await lMemberResult.Verify(member => member?.GameHandle != null, ApplicationErrors.MemberValidation.GameHandleNotSet)
                .Verify(member => member!.VerificationCodeExpiryDate > DateTimeOffset.Now, ApplicationErrors.MemberValidation.GameHandleVerificationCodeExpired)
                .Bind(member => _gameVerificationService.VerifyGameHandle(member!.GameHandle!, member.GameHandleVerificationCode, aCancellationToken))
                .Bind(_ => UpdateIsVerified(lMemberResult.Value!, aCancellationToken));
        }

        #endregion

        #endregion

        #region Private

        private async Task<IHttpResult<Member>> UpdateMemberStatus(Member aMember, MemberStatusEnum aMemberStatus, CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResult(aCancellationToken)
            .Tap(_ => aMember!.Status = aMemberStatus)
            .Bind(member => _memberRepository.Update(aMember, aCancellationToken));

        private async Task<IHttpResult<Member>> UpdateMemberProfile(Member aMember, MemberProfileUpdateDTO aMemberProfileDTO, CancellationToken aCancellationToken = default)
        => aMember.UpdateProfile(aMemberProfileDTO.GameHandle, aMemberProfileDTO.SpectrumCommunityMoniker)
            ? await _memberRepository.Update(aMember, aCancellationToken)
            : Result.SuccessHttp(aMember);

        private async Task<IHttpResult<Member>> UpdateMemberDisplayName(Member aMember, string aNewDisplayName, CancellationToken aCancellationToken = default)
        {
            aMember.DiscordGuildDisplayName = aNewDisplayName;
            return await _memberRepository.Update(aMember, aCancellationToken);
        }

        private async Task<IHttpResult<Member>> UpdateAvatar(Member aMember, string aNewAvatarUrl, CancellationToken aCancellationToken = default)
        {
            aMember.DiscordAvatarUrl = aNewAvatarUrl;
            return await _memberRepository.Update(aMember, aCancellationToken);
        }

        //Assigns the given roles to the given member and return a bool value representing this action updated the member permissiones(calculated from the permissions of the assinged roles)
        private async Task<IHttpResult<bool>> AssignRoleList(Member aMember, IEnumerable<DiscordRoleDTO> aAssignRoleList, CancellationToken aCancellationToken = default)
        {
            bool lIsPermissionsUpdated = default;
            var lMemberHighestRole = await _memberRoleService.GetHighestRole(aMember.Id, aCancellationToken);

            return await lMemberHighestRole
                .Bind(_ => _roleRepository.GetByIdListAsync(aAssignRoleList.Select(role => ulong.Parse(role.Id)).ToArray(), aCancellationToken))
                .Tap(roleToAssignList =>
                {
                    aMember.AssignRoleList(roleToAssignList.Select(role => role.Id));
                    lIsPermissionsUpdated = roleToAssignList.Any(role => role.IsApplicationRole() && role.Position > (lMemberHighestRole.Value?.Position ?? default));
                })
                .Bind(_ => _memberRepository.Update(aMember, aCancellationToken))
                .Map(_ => lIsPermissionsUpdated);
        }

        //Revokes the given roles to the given member and return a bool value representing this action updated the member permissiones(calculated from the permissions of the assinged roles)
        private async Task<IHttpResult<bool>> RevokeRoleList(Member aMember, IEnumerable<DiscordRoleDTO> aRevokeRoleList, CancellationToken aCancellationToken = default)
        {
            bool lIsPermissionsUpdated = default;
            var lMemberHighestRole = await _memberRoleService.GetHighestRole(aMember.Id, aCancellationToken);

            return await lMemberHighestRole
                .Bind(_ => _roleRepository.GetByIdListAsync(aRevokeRoleList.Select(role => ulong.Parse(role.Id)).ToArray(), aCancellationToken))
                .Tap(roleToRevokeList =>
                {
                    aMember.RevokeRoleList(roleToRevokeList.Select(role => role.Id));
                    lIsPermissionsUpdated = roleToRevokeList.Any(role => role.IsApplicationRole() && role.Position > (lMemberHighestRole.Value?.Position ?? default));
                })
                .Bind(_ => _memberRepository.Update(aMember, aCancellationToken))
                .Map(_ => lIsPermissionsUpdated);
        }

        //private async Task<IHttpResult<Member>> TryUpdateGameHandleVerifyCode(Member aMember, CancellationToken aCancellationToken = default)
        //=> aMember.RefreshVerifyCode()
        //    ? await _memberRepository.Update(aMember, aCancellationToken)
        //    : Result.SuccessHttp(aMember);

        private async Task<IHttpResult<PaginatedMemberListDTO>> GetPaginatedMemberListDTO(IEnumerable<Member> aMemberList, int aCurrentPage, int aPageSize)
        => await _memberRepository.GetCountAsync()
            .Map(memberCount => new PaginatedMemberListDTO(aCurrentPage, (int)Math.Ceiling((double)memberCount / aPageSize), aPageSize, memberCount, aMemberList.Select(member => member.ToDetailDto()).ToArray()));

        private async Task<IHttpResult<MemberDetailDTO>> UpdateIsVerified(Member aMember, CancellationToken aCancellationToken = default)
        => aMember.Verify()
            ? await _memberRepository.Update(aMember, aCancellationToken)
                .Map(member => member.ToDetailDto())
            : Result.Failure<MemberDetailDTO>(ApplicationErrors.MemberValidation.GameHandleVerificationFailed);

        private Member GetNewMemberEntity(CreateMemberDTO aCreateMemberDTO, DiscordProfileDTO aDiscordProfileDTO, IEnumerable<Role> aRoleList)
        => new(
            DiscordUserId: aCreateMemberDTO.DiscordCookieUserInfo.UserNameIdentifier,
            DiscordGuildDisplayName: GetDiscordGuildDisplayName(aCreateMemberDTO.DiscordCookieUserInfo, aDiscordProfileDTO),
            DiscordAvatarUrl: aDiscordProfileDTO.AvatarUrl,
            GameHandle: aCreateMemberDTO.SignUpData?.GameHandle,
            SpectrumCommunityMoniker: aCreateMemberDTO.SignUpData?.SpectrumCommunityMoniker,
            Roles: aRoleList.ToArray());

        //TO-DO: GSWB-27
        private string GetDiscordGuildDisplayName(DiscordCookieUserInfo aDiscordCookieUserInfo, DiscordProfileDTO aDiscordProfileDTO)
        {
            if (string.IsNullOrWhiteSpace(aDiscordProfileDTO.UserDisplayName) && !string.IsNullOrWhiteSpace(aDiscordCookieUserInfo.GivenName))
                return aDiscordCookieUserInfo.GivenName;

            return !string.IsNullOrWhiteSpace(aDiscordProfileDTO.UserDisplayName)
                   ? aDiscordProfileDTO.UserDisplayName
                   : aDiscordCookieUserInfo.UserName;
        }

        #endregion

    }

}
