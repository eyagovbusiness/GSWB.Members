using TGF.Common.ROP.Errors;

namespace Members.Domain.Errors
{
    public static partial class DomainErrors
    {
        public static partial class Validation
        {
            public static class VerifyCode
            {
                public static Error Null => new (
                    "VerifyCode.Null",
                    $"An error occurred when trying to access the VerifyCode of the member, the VerifyCode value was null."
                );
                public static Error Expired => new(
                    "VerifyCode.Expired",
                    $"The verify code has expired."
                );
            }
        }
    }
}
