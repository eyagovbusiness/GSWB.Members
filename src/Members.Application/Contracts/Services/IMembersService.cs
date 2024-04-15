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
        /// Get the paginated list of members based on the filters and sorted by the specified property.
        /// </summary>
        /// <returns>List of members and pagination metadata.</returns>
        public Task<IHttpResult<PaginatedMemberListDTO>> GetMemberList(
            int aPage, int aPageSize,
            string aSortBy,
            string? aDiscordNameFilter, string? aGameHandleFilter, ulong? aRoleIdFilter, bool? aIsVerifiedFilter,
            CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the total count of guild members.
        /// </summary>
        public Task<IHttpResult<int>> GetMembersCount(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Registers a new application member from a given DiscordUserId from the discord cookie.
        /// </summary>
        /// <returns>The new member created, otherwise error.</returns>
        public Task<IHttpResult<MemberDetailDTO>> AddNewMember(CreateMemberDTO aCreateMemberDTO, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the application member by its DiscordUserId.
        /// </summary>
        /// <returns>The found member associated with the provided DiscordUserId, otherwise error.</returns>
        public Task<IHttpResult<MemberDTO>> GetByDiscordUserId(ulong aDiscordUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the application member detailed by its DiscordUserId.
        /// </summary>
        /// <returns>The found member associated with the provided DiscordUserId, otherwise error.</returns>
        public Task<IHttpResult<MemberDetailDTO>> GetDetailByDiscordUserId(ulong aDiscordUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update the member by its DiscordUserId and the provided fields to be updated.
        /// </summary>
        /// <param name="aMemberProfileDTO"><see cref="MemberProfileUpdateDTO"/> with the member profile fields to be updated.</param>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberDetail(MemberProfileUpdateDTO aMemberProfileDTO, ulong aDiscordUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update the member's discord display name.
        /// </summary>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <param name="aNewDisplayName">New discord display name in the server.</param>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberDiscordDisplayName(ulong aDiscordUserId, string aNewDisplayName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update the member's discord avatar image.
        /// </summary>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <param name="aNewAvatarUrl">New member avatar url.</param>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberAvatar(ulong aDiscordUserId, string aNewAvatarUrl, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Assign a list of roles to a certain member identified by its DiscordUserId.
        /// </summary>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <param name="aAssignRoleList">List of roles to be assigned to the given member identified by its DiscordUserId.</param>
        /// <returns>True if the member's highest application permissions were updated after assigning the new roles, otherwise false.</returns>
        public Task<IHttpResult<bool>> AssignMemberRoleList(ulong aDiscordUserId, IEnumerable<DiscordRoleDTO> aAssignRoleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Revoke a list of roles to a certain member identified by its DiscordUserId.
        /// </summary>
        /// <param name="aDiscordUserId">DiscordUserId related with the member.</param>
        /// <param name="aRevokeRoleList">List of roles to be revoked to the given member identified by its DiscordUserId.</param>
        /// <returns>True if the member's highest application permissions were updated after revoking the specified roles, otherwise false.</returns>
        public Task<IHttpResult<bool>> RevokeMemberRoleList(ulong aDiscordUserId, IEnumerable<DiscordRoleDTO> aRevokeRoleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Delete application member detailed by its DiscordUserId.
        /// </summary>
        /// <returns>The found member associated with the provided DiscordUserId, otherwise error.</returns>
        public Task<IHttpResult<Member>> DeleteMember(ulong aDiscordUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update a given member's status by its DiscordUserId.
        /// </summary>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberStatus(ulong aDiscordUserId, MemberStatusEnum aMemberStatus, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get game handle verification info related data for a given member by its DiscordUserId as <see cref="MemberVerifyDTO"/>.
        /// </summary>
        /// <returns>The current <see cref="MemberVerifyInfoDTO"/> or error if any.</returns>
        public Task<IHttpResult<MemberVerifyInfoDTO>> Get_GetVerifyInfo(ulong aDiscordUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Verifies the GameHandle of the authenticated member, wit will succeed if the member's GameHandleVerificationCode is present at the game handle's profile bio from "https://robertsspaceindustries.com/citizens/{Member.GameHandle}".
        /// </summary>
        /// <returns><see cref="MemberDetailDTO"/> with the refreshed member information after the verify attempt.</returns>
        public Task<IHttpResult<MemberDetailDTO>> VerifyGameHandle(ulong aDiscordUserId, CancellationToken aCancellationToken = default);

    }
}
