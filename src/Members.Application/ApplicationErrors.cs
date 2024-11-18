using System.Net;
using TGF.Common.ROP.Errors;

namespace Members.Application
{
    public static class ApplicationErrors
    {
        public static class Members
        {
            public static HttpError DiscordAccountAlreadyRegistered => new(
            new Error("MembersDb.DiscordAccountAlreadyRegistered",
                "A member account already exists under this DiscordUserId."),
            HttpStatusCode.BadRequest);

        }

        public static class Guilds
        {
            public static HttpError NotAdded => new(
            new Error("GuildsDb.NotAdded",
                "The guild could not be added in the DB."),
            HttpStatusCode.InternalServerError);

        }

    }
}
