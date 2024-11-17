using Ardalis.Specification;
using Members.Domain.Entities;

namespace Members.Application.Specifications
{
    internal class RolesOfGuildIdSpec: Specification<Role>
    {
        public RolesOfGuildIdSpec(
            ulong guildId
        )
        {
                Query.Where(role =>role.GuildId ==  guildId);
        }
    }
}
