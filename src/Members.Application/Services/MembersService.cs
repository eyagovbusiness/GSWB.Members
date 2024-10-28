using Common.Application.Contracts.Services;
using Common.Application.DTOs.Auth;
using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Application.Services
{
    public class MembersService : IMembersService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ISwarmBotCommunicationService _SwarmBotCommunicationService;
        private readonly IGameVerificationService _gameVerificationService;
        public MembersService(
            IMemberRepository aMemberRepository,
            ISwarmBotCommunicationService aSwarmBotCommunicationService,
            IGameVerificationService aGameVerificationService)
        {
            _memberRepository = aMemberRepository;
            _SwarmBotCommunicationService = aSwarmBotCommunicationService;
            _gameVerificationService = aGameVerificationService;
        }

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


        public async Task<IHttpResult<MemberDTO>> GetByDiscordUserId(Guid id, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByIdAsync(id, aCancellationToken)
            .Map(member => member!.ToDto());

        public async Task<IHttpResult<MemberDetailDTO>> GetDetailByDiscordUserId(Guid id, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByIdAsync(id, aCancellationToken)
            .Map(member => member!.ToDetailDto());

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberDetail(MemberProfileUpdateDTO aMemberProfileDTO, Guid id, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByIdAsync(id, aCancellationToken)
            .Bind(member => UpdateMemberProfile(member!, aMemberProfileDTO, aCancellationToken))
            .Map(member => member.ToDetailDto());

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberDiscordDisplayName(ulong userId, ulong guildId, string aNewDisplayName, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByGuildAndUserIdsAsync(guildId, userId, aCancellationToken)
            .Bind(member => UpdateMemberDisplayName(member!, aNewDisplayName, aCancellationToken))
            .Map(member => member.ToDetailDto());

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberAvatar(ulong userId, ulong guildId, string aNewAvatarUrl, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByGuildAndUserIdsAsync(guildId, userId, aCancellationToken)
            .Bind(member => UpdateAvatar(member!, aNewAvatarUrl, aCancellationToken))
            .Map(member => member.ToDetailDto());


        public async Task<IHttpResult<Member>> DeleteMember(Guid id, CancellationToken aCancellationToken = default)
         => await _memberRepository.GetByIdAsync(id, aCancellationToken)
            .Bind(member => _memberRepository.Delete(member!, aCancellationToken));

        public async Task<IHttpResult<MemberDetailDTO>> UpdateMemberStatus(ulong userId, ulong guildId, MemberStatusEnum aMemberStatus, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByGuildAndUserIdsAsync(guildId, userId, aCancellationToken)
            .Bind(member => UpdateMemberStatus(member!, aMemberStatus, aCancellationToken))
            .Map(member => member.ToDetailDto());

        #region Verify

        public async Task<IHttpResult<MemberVerificationStateDTO>> Get_GetVerifyInfo(Guid id, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByIdAsync(id, aCancellationToken)
            .Bind(member => TryUpdateGameHandleVerifyCode(member!, aCancellationToken))
            .Map(member => new MemberVerificationStateDTO(member.IsGameHandleVerified, member!.GameHandleVerificationCode, member.VerificationCodeExpiryDate));

        public async Task<IHttpResult<MemberDetailDTO>> VerifyGameHandle(Guid id, CancellationToken aCancellationToken = default)
        {
            var lMemberResult = await _memberRepository.GetByIdAsync(id, aCancellationToken);
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

        private async Task<IHttpResult<Member>> TryUpdateGameHandleVerifyCode(Member aMember, CancellationToken aCancellationToken = default)
        => aMember.TryRefreshGameHandleVerificationCode()
            ? await _memberRepository.Update(aMember, aCancellationToken)
            : Result.SuccessHttp(aMember);

        private async Task<IHttpResult<PaginatedMemberListDTO>> GetPaginatedMemberListDTO(IEnumerable<Member> aMemberList, int aCurrentPage, int aPageSize)
        => await _memberRepository.GetCountAsync()
            .Map(memberCount => new PaginatedMemberListDTO(aCurrentPage, (int)Math.Ceiling((double)memberCount / aPageSize), aPageSize, memberCount, aMemberList.Select(member => member.ToDetailDto()).ToArray()));

        private async Task<IHttpResult<MemberDetailDTO>> UpdateIsVerified(Member aMember, CancellationToken aCancellationToken = default)
        => aMember.Verify()
            ? await _memberRepository.Update(aMember, aCancellationToken)
                .Map(member => member.ToDetailDto())
            : Result.Failure<MemberDetailDTO>(ApplicationErrors.MemberValidation.GameHandleVerificationFailed);


        #endregion

    }

}
