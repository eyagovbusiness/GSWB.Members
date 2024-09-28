using Members.Application;
using Members.Application.Contracts.Repositories;
using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB.Repository;
using TGF.Common.ROP.HttpResult;

namespace Members.Infrastructure.Repositories
{
    public class GuildRepository(MembersDbContext aContext, ILogger<GuildRepository> aLogger)
        : RepositoryBase<GuildRepository, MembersDbContext>(aContext, aLogger), IGuildRepository, ISortRepository
    {

    }

}
