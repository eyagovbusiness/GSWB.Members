using Members.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    /// <summary>
    /// Represents a verification code expedited for a given member in order to support some external verification with the system. 
    /// </summary>
    /// <remarks>Used to verify RSI account by pasting this generated code for the member in the RSI profile.</remarks>
    public class VerifyCode : Entity<Guid>
    {

        /// <summary>
        /// 6-digit verification code used for external member verification. Used to verify RSI account by pasting this generated code for the member in the RSI profile.
        /// </summary>
        [MaxLength(Validation.InvariantConstants.VerifyCodeLength)]
        [MinLength(Validation.InvariantConstants.VerifyCodeLength)]
        public required string Code { get; set; } = "000000";

        /// <summary>
        /// The expiry date for this verification code. 
        /// </summary>
        public DateTimeOffset ExpiryDate { get; set; } = DateTime.Now;

        [SetsRequiredMembers]
        internal VerifyCode()
        {
            TryRefresh();
        }

        #region BusinessLogic
        /// <summary>
        /// Refresh the Code and expiry date if the current code has expired already.
        /// </summary>
        /// <returns>True if it was refreshed, otherwise if it was not expired yet and nothign was changed returns false.</returns>
        internal bool TryRefresh()
        {
            if (ExpiryDate > DateTimeOffset.Now)
                return false;

            Code = GetNewCode();
            ExpiryDate = GetExpiryDate();

            return true;
        }
        private static string GetNewCode() => new Random().Next(100000, 999999).ToString();
        private static DateTimeOffset GetExpiryDate() => DateTimeOffset.Now.AddMinutes(InvariantConstants.VerifyCodeLifetimeInMinutes);
        #endregion

    }
}
