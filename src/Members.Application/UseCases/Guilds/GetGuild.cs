using Common.Application.DTOs.Guilds;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Guilds
{
    /// <summary>
    /// Use case to get a given guild from its Id.
    /// </summary>
    public class GetGuild(IGuildRepository guildRepository)
        : IUseCase<IHttpResult<GuildDTO>, string>
    {
        public async Task<IHttpResult<GuildDTO>> ExecuteAsync(string guildId, CancellationToken cancellationToken = default)
        => await guildRepository.GetByIdAsync(ulong.Parse(guildId), cancellationToken)
        .Map(guild => guild.ToDto());
    }
}
