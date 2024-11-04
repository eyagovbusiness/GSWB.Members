using Members.Domain.Entities;
using TGF.CA.Domain.Contracts.Repositories.EntityRepository;

namespace Members.Domain.Contracts.Repositories.ReadOnly
{
    public interface IRoleQueryRepository : IEntityQueryRepository<Role, ulong>
    {
    }
}
