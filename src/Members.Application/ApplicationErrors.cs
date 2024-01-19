using System.Net;
using TGF.Common.ROP.Errors;

namespace Members.Application
{
    public static class ApplicationErrors
    {
        public class MemberValidation
        {

            public static HttpError GameHandleNotSet => new(
            new Error("Member.ValidationError.GameHandleNotSet",
                "The member did not set a GameHandle yet."),
            HttpStatusCode.BadRequest);

            public static HttpError GameHandleVerificationFailed => new(
            new Error("Member.ValidationError.GameHandleVerificationFailed",
                "The GameHandle verification process failed, this may be because it was already verified, the verification failed or the server could not handle the verification process at this moment."),
            HttpStatusCode.InternalServerError);

            public static HttpError GameHandleVerificationCodeExpired => new(
            new Error("Member.ValidationError.GameHandleVerificationCodeExpired",
                "The GameHandle verification code has expired already, please request a code refresh and try again with the new one."),
            HttpStatusCode.BadRequest);

        }

        public class Members
        {
            public static HttpError DiscordAccountAlreadyRegistered => new(
            new Error("MembersDb.DiscordAccountAlreadyRegistered",
                "A member account already exists under this DiscordUserId."),
            HttpStatusCode.BadRequest);

        }

    }
}
