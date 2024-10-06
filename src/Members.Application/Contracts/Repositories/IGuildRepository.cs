using Members.Domain.Entities;
using TGF.CA.Domain.Contracts.Repositories;

namespace Members.Application.Contracts.Repositories
{
    public interface IGuildRepository : IRepositoryBase<Guild, ulong>
    {
    }
}
