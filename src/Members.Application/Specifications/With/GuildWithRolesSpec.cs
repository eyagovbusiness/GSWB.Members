using Ardalis.Specification;
using Members.Domain.Entities;

namespace Members.Application.Specifications.With
{
    public class GuildWithRolesSpec : Specification<Guild>
    {
        public GuildWithRolesSpec() {
            Query.Include(x => x.Roles);
        }
    }
}
