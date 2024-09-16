using TGF.Common.ROP.Errors;

namespace Members.Domain.Errors
{
    public static partial class DomainErrors
    {
        public static partial class Validation
        {
            public static class Member
            {
                public static Error AlreadyVerified => new(
                    "Member.AlreadyVerified",
                    $"An error occurred when trying to access the VerifyCode of the member, the VerifyCode value was null."
                );
            }
        }
    }
}
