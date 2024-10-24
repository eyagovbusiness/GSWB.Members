using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using Members.Application.Mapping;
using TGF.Common.ROP.Result;
using Common.Application.DTOs.Guilds;
using Members.Domain.Contracts.Repositories;
using Common.Application.Contracts.Services;
using Members.Domain.ValueObjects.Role;
using Common.Application.DTOs.Discord;
using Members.Domain.Validation;

namespace Members.Application.UseCases.Guilds
{
    /// <summary>
    /// Use case to add a new guild
    /// </summary>
    public class AddGuildUseCase(IGuildRepository guildRepository, ISwarmBotCommunicationService swarmBotCommunicationService)
        : IUseCase<IHttpResult<GuildDTO>, GuildDTO>
    {
        public async Task<IHttpResult<GuildDTO>> ExecuteAsync(GuildDTO guildDTO, CancellationToken cancellationToken = default)
        {
            var exsitingGuildResult = await guildRepository.GetByIdAsync(ulong.Parse(guildDTO.Id), cancellationToken);
            if(!exsitingGuildResult.IsSuccess)
            {
                Guild newGuild = default!;
                return await guildRepository.AddAsync(new Guild(guildDTO.Id, guildDTO.Name, guildDTO.IconUrl), cancellationToken)
                .Tap(guild => newGuild = guild)
                .Bind(guild => swarmBotCommunicationService.GetGuildDiscordRoleList(guild.Id.ToString(), cancellationToken))
                //.Bind(EnsureGuildSwarmAdminRole)
                .Bind(roleDTOList => newGuild.AddRoles(roleDTOList.Select(roleDTO => new DiscordRoleValues(ulong.Parse(roleDTO.RoleId), roleDTO.Name, roleDTO.Position))))
                .Bind(guild => guildRepository.UpdateAsync(guild, cancellationToken))
                .Map(guild => guild.ToDto());
            }

            if (exsitingGuildResult.IsSuccess && exsitingGuildResult.Value != null!)
                return exsitingGuildResult.Map(guild => guild.ToDto());

            return Result.Failure<GuildDTO>(ApplicationErrors.Guilds.NotAdded);
        }
        //public async Task<IHttpResult<IEnumerable<DiscordRoleDTO>>> EnsureGuildSwarmAdminRole(IEnumerable<DiscordRoleDTO> discordRoles)
        //{
        //    if (!discordRoles.Any(role => role.Name == InvariantConstants.Role_Name_IsGuildSwarmAdmin))
        //    swarmBotCommunicationService.EnsureGuildSwarmAdminRole //this will create the role and get the updated list with the role oncluded
        //}

    }
} 
