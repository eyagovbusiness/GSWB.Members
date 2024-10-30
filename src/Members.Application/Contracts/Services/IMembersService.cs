using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;

namespace Members.Application
{
    public interface IMembersService
    {

        /// <summary>
        /// Get the list of members from the provided members id list.
        /// </summary>
        /// <returns>List of members matching the id list.</returns>
        public Task<IHttpResult<IEnumerable<MemberDetailDTO>>> GetMembersByIdList(IEnumerable<Guid> aMemberIdList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the total count of guild members.
        /// </summary>
        public Task<IHttpResult<int>> GetMembersCount(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the application member by its Id.
        /// </summary>
        /// <returns>The found member associated with the provided Id, otherwise error.</returns>
        public Task<IHttpResult<MemberDTO>> GetByDiscordUserId(Guid id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the application member detailed by its Id.
        /// </summary>
        /// <returns>The found member associated with the provided Id, otherwise error.</returns>
        public Task<IHttpResult<MemberDetailDTO>> GetDetailByDiscordUserId(Guid id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update the member by its DiscordUserId and the provided fields to be updated.
        /// </summary>
        /// <param name="aMemberProfileDTO"><see cref="MemberProfileUpdateDTO"/> with the member profile fields to be updated.</param>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberDetail(MemberProfileUpdateDTO aMemberProfileDTO, Guid id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update the member's discord display name.
        /// </summary>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <param name="aNewDisplayName">New discord display name in the server.</param>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberDiscordDisplayName(ulong aUserId, ulong GuildId,  string aNewDisplayName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update the member's discord avatar image.
        /// </summary>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <param name="aNewAvatarUrl">New member avatar url.</param>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberAvatar(ulong userId, ulong guildId, string aNewAvatarUrl, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Delete application member detailed by its Id.
        /// </summary>
        /// <returns>The found member associated with the provided Id, otherwise error.</returns>
        public Task<IHttpResult<Member>> DeleteMember(Guid id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update a given member's status by its Id.
        /// </summary>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberStatus(ulong userId, ulong guildId, MemberStatusEnum aMemberStatus, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get game handle verification info related data for a given member by its Id as <see cref="MemberVerifyDTO"/>.
        /// </summary>
        /// <returns>The current <see cref="MemberVerifyInfoDTO"/> or error if any.</returns>
        public Task<IHttpResult<MemberVerificationStateDTO>> Get_GetVerifyInfo(Guid id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Verifies the GameHandle of the authenticated member, wit will succeed if the member's GameHandleVerificationCode is present at the game handle's profile bio from "https://robertsspaceindustries.com/citizens/{Member.GameHandle}".
        /// </summary>
        /// <returns><see cref="MemberDetailDTO"/> with the refreshed member information after the verify attempt.</returns>
        public Task<IHttpResult<MemberDetailDTO>> VerifyGameHandle(Guid id, CancellationToken aCancellationToken = default);

    }
}
