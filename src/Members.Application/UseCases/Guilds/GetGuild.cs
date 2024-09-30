using Common.Application.DTOs.Guilds;
using Members.Application.Contracts.Repositories;
using Members.Application.Mapping;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Guilds
{
    public class GetGuild(IGuildRepository guildRepository)
        : IUseCase<IHttpResult<GuildDTO>, string>
    {
        public async Task<IHttpResult<GuildDTO>> ExecuteAsync(string guildId, CancellationToken aCancellationToken = default)
        => await guildRepository.GetByIdAsync(ulong.Parse(guildId), aCancellationToken)
        .Map(guild => guild.ToDto());
    }
}
