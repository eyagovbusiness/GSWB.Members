using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;

namespace Members.Application
{
    public interface IMembersService
    {

        /// <summary>
        /// Delete application member detailed by its Id.
        /// </summary>
        /// <returns>The found member associated with the provided Id, otherwise error.</returns>
        public Task<IHttpResult<Member>> DeleteMember(MemberKey id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Update a given member's status by its Id.
        /// </summary>
        /// <returns>The updated member DTO.</returns>
        public Task<IHttpResult<MemberDetailDTO>> UpdateMemberStatus(ulong userId, ulong guildId, MemberStatusEnum aMemberStatus, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get game handle verification info related data for a given member by its Id as <see cref="MemberVerifyDTO"/>.
        /// </summary>
        /// <returns>The current <see cref="MemberVerifyInfoDTO"/> or error if any.</returns>
        public Task<IHttpResult<MemberVerificationStateDTO>> Get_GetVerifyInfo(MemberKey id, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Verifies the GameHandle of the authenticated member, wit will succeed if the member's GameHandleVerificationCode is present at the game handle's profile bio from "https://robertsspaceindustries.com/citizens/{Member.GameHandle}".
        /// </summary>
        /// <returns><see cref="MemberDetailDTO"/> with the refreshed member information after the verify attempt.</returns>
        public Task<IHttpResult<MemberDetailDTO>> VerifyGameHandle(MemberKey id, CancellationToken aCancellationToken = default);

    }
}
