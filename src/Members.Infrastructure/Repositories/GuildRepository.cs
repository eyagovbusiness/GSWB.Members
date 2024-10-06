using Members.Application.Contracts.Repositories;
using Members.Domain.Entities;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB.Repository;

namespace Members.Infrastructure.Repositories
{
    public class GuildRepository(MembersDbContext aContext, ILogger<GuildRepository> aLogger)
        : RepositoryBase<GuildRepository, MembersDbContext, Guild, ulong>(aContext, aLogger), IGuildRepository, ISortRepository
    {

    }

}
