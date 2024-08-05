
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
        public virtual VerifyCode VerifyCode { get; set; }


    }
}
