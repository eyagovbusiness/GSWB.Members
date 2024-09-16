using Members.Domain.Validation;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Domain.Entities
{
    public partial class Member
    {
        /// <summary>
        /// The Roles assigned to this member.
        /// </summary>
        public virtual ICollection<MemberRole> Roles { get; set; } = [];

        /// <summary>
        /// The verification code generated for this member enitiy.
        /// </summary>
        public virtual VerifyCode? VerifyCode { get; set; }

        /// <summary>
        /// Verifies the GameHandle if the verify code has not expired yet and the member was not verified yet, setting IsGameHandleVerified to true. If the member was already verified it return an error.
        /// </summary>
        /// <returns>Success if IsGameHandleVerified is updated to true, otherwise error.</returns>
        public IHttpResult<Unit> Verify(MemberVerificationValidator aMemberVerificationValidator)
        => Result.ValidationResult(aMemberVerificationValidator.Validate(this))
        .Tap(_ => IsGameHandleVerified = true);

        /// <summary>
        /// Update game handle verification code ONLY if the game handle was not verified already and the code has expired already.
        /// </summary>
        /// <returns>The instance fo the Member with the VerifyCode successfully refreshed, otherwise error.</returns>
        public IHttpResult<Member> RefreshVerifyCode(MemberVerificationValidator aMemberVerificationValidator)
        => Result.ValidationResult(aMemberVerificationValidator.Validate(this))
        .Tap(_ => 
        {
            VerifyCode!.Code = new Random().Next(100000, 999999).ToString();
            VerifyCode.ModifiedAt = DateTimeOffset.Now;//update modified at now to update the virtual expiry date property, when the entitiy will be savedin DB it will updated to the save time.
        })
        .Map(_ => this);

        /// <summary>
        /// Updates basic member profile fields, GameHandle and SpectrumCommunityMoniker.
        /// </summary>
        /// <param name="aGameHandle">New value to update the GameHandle with.</param>
        /// <param name="aSpectrumCommunityMoniker">New value to update the SpectrumCommunityMoniker with.</param>
        /// <returns>True if any field was updated, otherwise false.</returns>
        public bool UpdateProfile(string? aGameHandle, string? aSpectrumCommunityMoniker)
        {
            bool lHasBeenUpdated = false;

            if (aGameHandle != null && GameHandle != aGameHandle)
            {
                GameHandle = aGameHandle;
                IsGameHandleVerified = false;
                lHasBeenUpdated = true;
            }

            if (aSpectrumCommunityMoniker != null && SpectrumCommunityMoniker != aSpectrumCommunityMoniker)
            {
                SpectrumCommunityMoniker = aSpectrumCommunityMoniker;
                lHasBeenUpdated = true;
            }

            return lHasBeenUpdated;
        }

        /// <summary>
        /// Add to the Roles list new MemberRole objects from the provided Role.Id list
        /// </summary>
        /// <param name="aRoleIdList">List of Role.Id to assign to this member.</param>
        public void AssignRoleList(IEnumerable<ulong> aRoleIdList)
        => Roles = Roles.Union(aRoleIdList.Select(roleId => new MemberRole() { Member = this, RoleId = roleId }))
            .ToList();

        /// <summary>
        /// Removes MemberRole objects from the Roles list based on the provided Role.Id list.
        /// </summary>
        /// <param name="aRoleIdList">List of Role.Id to revoke from this member.</param>
        public void RevokeRoleList(IEnumerable<ulong> aRoleIdList)
        => Roles = Roles.Where(role => !aRoleIdList.Contains(role.RoleId))
            .ToList();

    }
}
