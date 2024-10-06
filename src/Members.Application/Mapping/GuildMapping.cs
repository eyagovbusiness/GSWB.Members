

using Common.Application.DTOs.Guilds;
using Members.Domain.Entities;

namespace Members.Application.Mapping
{
    public static class GuildMapping
    {
        public static GuildDTO ToDto(this Guild guild)
        => new(guild.Id.ToString(), guild.Name, guild.IconUrl);
    }
}
