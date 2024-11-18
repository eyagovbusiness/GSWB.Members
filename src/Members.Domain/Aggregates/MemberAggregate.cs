using Members.Domain.Validation.Guild;
using System.Collections.Immutable;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;

namespace Members.Domain.Entities
{
    public partial class Member
    {
        public virtual ICollection<MemberRole> Roles { get; protected set; } = [];
        public virtual VerifyCode? VerifyCode { get; protected set; } = default;

        #region Roles
        internal async Task<IHttpResult<Member>> AssignRoles(IEnumerable<ulong> roleIdList, GuildRoleIdValidator guildRoleIdValidator)
        {
            var lValidationResult = await guildRoleIdValidator.ValidateAsync(new GuildRoleIdValidationData(GuildId, roleIdList));

            if (!lValidationResult.IsValid)
                return Result.Failure<Member>(lValidationResult.Errors
                    .Select(e => ValidateSwitchExtensions.GetValidationError(e.ErrorCode, e.ErrorMessage))
                    .ToImmutableArray());

            roleIdList.Where(roleId => !Roles.Any(role => role.RoleId == roleId))
            .ToList()
            .ForEach(roleId => Roles.Add(new MemberRole() { Member = this, RoleId = roleId }));

            return Result.SuccessHttp(this);
        }

        internal IHttpResult<Member> RevokeRoles(IEnumerable<ulong> roleIdList)
        => Result.SuccessHttp(roleIdList)
            .Map(roleIdList => Roles.Where(memberRole => roleIdList.Contains(memberRole.RoleId)))
            .Tap(memberRoleList => Roles = Roles.Except(memberRoleList).ToList())
            .Map(_ => this);
        #endregion

        #region VerifyCode 
        /// <summary>
        /// Verifies the GameHandle if the VerificationCodeExpiryDate has not expired yet, setting IsGameHandleVerified to true.
        /// </summary>
        /// <returns>True in case IsGameHandleVerified was false and got updated to tue, otherwise false.</returns>
        public IHttpResult<Member> Verify()
        {
            if (VerifyCode?.ExpiryDate == null)
                throw new Exception("Member.VerifyCode was null, did you forget to use include in the db query?");

            if (!IsGameHandleVerified && VerifyCode.ExpiryDate > DateTimeOffset.Now)
                IsGameHandleVerified = true;

            return Result.SuccessHttp(this);
        }

        /// <summary>
        /// Ensures the verify code is set.
        /// </summary>
        public IHttpResult<Member> EnsureGameHandleVerificationCode()
        {
            if (this.VerifyCode! == null!)
                this.VerifyCode = new VerifyCode();
            return Result.SuccessHttp(this);
        }

        /// <summary>
        /// Update game handle verification code ONLY if the game handle was not verified already and the code has expired already.
        /// </summary>
        /// <returns>True if GameHandleVerificationCode was updated, otherwise false.</returns>
        public IHttpResult<Member> TryRefreshGameHandleVerificationCode()
        {
            if (VerifyCode! == null!)
                throw new Exception("Member.VerifyCode was null, did you forget to use include in the db query?");

            if (!IsGameHandleVerified)
                this.VerifyCode.TryRefresh();

            return Result.SuccessHttp(this);
        }
        #endregion

    }
}
