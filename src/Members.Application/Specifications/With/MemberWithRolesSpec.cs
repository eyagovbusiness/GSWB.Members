using Ardalis.Specification;
using Members.Domain.Entities;

namespace Members.Application.Specifications.With
{
    public class MemberWithRolesSpec: Specification<Member>
    {
        public MemberWithRolesSpec() {
            Query.Include(x => x.Roles);
        }
    }
}
