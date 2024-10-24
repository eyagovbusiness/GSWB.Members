using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using Members.Application.Mapping;
using TGF.Common.ROP.Result;
using Common.Application.DTOs.Guilds;
using Members.Domain.Contracts.Repositories;

namespace Members.Application.UseCases.Guilds
{
    /// <summary>
    /// Use case to delete a given guild from its Id and all the guild associated data.
    /// </summary>
    public class DeleteGuild(IGuildRepository guildRepository)
        : IUseCase<IHttpResult<GuildDTO>, string>
    {
        public async Task<IHttpResult<GuildDTO>> ExecuteAsync(string guildId, CancellationToken cancellationToken = default)
        => await guildRepository.GetByIdAsync(ulong.Parse(guildId), cancellationToken)
        .Bind(guild => guildRepository.DeleteAsync(guild, cancellationToken))
        .Map(guild => guild.ToDto());

    }
} 
