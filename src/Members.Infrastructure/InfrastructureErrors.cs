using System.Net;
using TGF.Common.ROP.Errors;

namespace Members.Infrastructure
{
    internal static class InfrastructureErrors
    {

        public static class MembersDb
        {
            public static HttpError NotFoundId => new(
            new Error("MembersDb.NotFoundId",
                "No registered member was found under the specified Id."),
            HttpStatusCode.NotFound);
            public static HttpError NotFoundDiscordUserId => new(
            new Error("MembersDb.NotFoundDiscordUserId",
                "No registered member was found under the specified DiscordUserId."),
            HttpStatusCode.NotFound);
        }

        public static class RolesDb
        {
            public static HttpError ListNotFoundId => new(
            new Error("Roles.List.NotFoundId",
                "Error, at least one Role ID was not found in DB from the provided role ID list."),
            HttpStatusCode.NotFound);
        }

        public static class GameHandleVerify
        {
            public static HttpError CodeNotFound => new(
            new Error("GameHandleVerify.CodeNotFound",
                "Verification code not found in the game profile."),
            HttpStatusCode.NotFound);
            public static HttpError FetchHandleFailed => new(
            new Error("GameHandleVerify.FetchHandleFailed",
                "Failed to fetch the game handle. This may be because the provided game handle does not exist or the RSI webpage resource is down at the moment."),
            HttpStatusCode.InternalServerError);
            public static HttpError ErrorReadingBio => new(
            new Error("GameHandleVerify.ErrorReadingBio",
                "An error occured while trying to read the profile's bio under the provided game handle."),
            HttpStatusCode.InternalServerError);
        }

    }
}
