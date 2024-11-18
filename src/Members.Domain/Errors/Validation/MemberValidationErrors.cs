using TGF.Common.ROP.Errors;

namespace Members.Domain.Errors.Validation
{
    public static partial class DomainErrors
    {
        public static partial class Validation
        {
            public static class Member
            {
                public static Error GamehandleAlreadyVerified => new(
                     "Validation.Member.GamehandleAlreadyVerified",
                     $"The member has verified already the game handle"
                 );
                public static Error VerifyCodeExpired => new(
                     "Validation.Member.VerifyCodeExpired",
                     $"The verification code has expired"
                 );
                public static Error VerifyCodeNotValidFormat => new(
                     "Validation.Member.VerifyCodeNotValidFormat",
                     $"The verification code is not valid in a valid fromat, but be not empty {6} numeric characters"
                 );
                public static Error GameHandleNotSet => new(
                    "Member.ValidationError.GameHandleNotSet",
                    "The member did not set a GameHandle yet."
                );
            }
        }
    }
}
