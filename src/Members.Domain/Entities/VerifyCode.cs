using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        /// Discord user ID from OAuth with discord
        /// </summary>
        public required Member Member { get; set; }

        /// <summary>
        ///  6-digit code used for external member verification. Used to verify RSI account by pasting this generated code for the member in the RSI profile.
        /// </summary>
        [MaxLength(6)]
        public required string Code { get; set; } = "000000";

        /// <summary>
        /// The expiry date for this verification code. 
        /// </summary>
        [NotMapped]
        public DateTimeOffset ExpiryDate => ModifiedAt.AddMinutes(1);
    }
}
