using Common.Domain.ValueObjects;

namespace Members.Domain.Entities
{

    public partial class Member
    {
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
        /// Verifies the GameHandle if the VerificationCodeExpiryDate has not expired yet, setting IsGameHandleVerified to true.
        /// </summary>
        /// <returns>True in case IsGameHandleVerified was false and got updated to tue, otherwise false.</returns>
        public bool Verify()
        {
            if (!IsGameHandleVerified
                && VerificationCodeExpiryDate > DateTimeOffset.Now)
            {
                IsGameHandleVerified = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update game handle verification code ONLY if the game handle was not verified already and the code has expired already.
        /// </summary>
        /// <returns>True if GameHandleVerificationCode was updated, otherwise false.</returns>
        public bool TryRefreshGameHandleVerificationCode()
        {
            if (IsGameHandleVerified || VerificationCodeExpiryDate > DateTimeOffset.Now)
                return false;
            GameHandleVerificationCode = new Random().Next(100000, 999999).ToString();
            VerificationCodeExpiryDate = DateTimeOffset.Now.AddMinutes(1);
            return true;
        }

        ///// <summary>
        ///// Get this member's highest application role, which determines this member's permissions within the application.
        ///// </summary>
        //public Role? GetHighestRole()
        //    => Roles.Where(r => r.IsApplicationRole()).MaxBy(role => role.Position);

        ///// <summary>
        ///// Get this member's permissions, calculated by Bitwise OR to accumulate permissions from each permissions of all roles assigned to the member.
        ///// </summary>
        //// Method to calculate total permissions
        //public PermissionsEnum CalculatePermissions()
        //{
        //    PermissionsEnum lTotalPermissions = PermissionsEnum.None;

        //    foreach (var lRole in Roles)
        //        lTotalPermissions |= lRole.Permissions; // Bitwise OR to accumulate permissions

        //    return lTotalPermissions;
        //}

    }
}
