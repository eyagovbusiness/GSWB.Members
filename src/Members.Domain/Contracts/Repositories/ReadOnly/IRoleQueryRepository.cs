using Members.Domain.Entities;
using TGF.CA.Domain.Contracts.Repositories;

namespace Members.Domain.Contracts.Repositories.ReadOnly
{
    public interface IRoleQueryRepository : IQueryRepository<Role, ulong>
    {
    }
}
