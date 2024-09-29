using Members.Application.Contracts.Repositories;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using Members.Application.Mapping;
using TGF.Common.ROP.Result;
using Common.Application.DTOs.Guilds;

namespace Members.Application.UseCases.Guilds
{
    public class AddGuildUseCase(IGuildRepository guildRepository)
        : IUseCase<IHttpResult<GuildDTO>, GuildDTO>
    {
        public async Task<IHttpResult<GuildDTO>> ExecuteAsync(GuildDTO guildDTO, CancellationToken aCancellationToken = default)
        {
            var exsitingGuildResult = await guildRepository.GetByIdAsync(ulong.Parse(guildDTO.Id), aCancellationToken);
            if(!exsitingGuildResult.IsSuccess)
                return await guildRepository.AddAsync(new Guild(guildDTO.Id, guildDTO.Name, guildDTO.IconUrl), aCancellationToken)
                    .Map(guild => guild.ToDto());
            if (exsitingGuildResult.IsSuccess && exsitingGuildResult.Value != null)
                return exsitingGuildResult.Map(guild => guild.ToDto());
            return Result.Failure<GuildDTO>(ApplicationErrors.Guilds.NotAdded);
        }
    }
} 
