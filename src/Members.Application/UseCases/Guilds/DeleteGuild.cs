using Members.Application.Contracts.Repositories;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using Members.Application.Mapping;
using TGF.Common.ROP.Result;
using Common.Application.DTOs.Guilds;

namespace Members.Application.UseCases.Guilds
{
    public class DeleteGuild(IGuildRepository guildRepository)
        : IUseCase<IHttpResult<GuildDTO>, string>
    {
        public async Task<IHttpResult<GuildDTO>> ExecuteAsync(string guildId, CancellationToken cancellationToken = default)
        => await guildRepository.GetByIdAsync(ulong.Parse(guildId), cancellationToken)
        .Bind(guild => guildRepository.DeleteAsync(guild, cancellationToken))
        .Map(guild => guild.ToDto());

    }
} 
