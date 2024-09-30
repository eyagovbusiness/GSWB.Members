using Common.Application.DTOs.Guilds;
using Members.Application.Contracts.Repositories;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Guilds
{
    //public class ListUserGuilds(IGuildRepository guildRepository, IMemberRepository memberRepository)
    //    : IUseCase<IHttpResult<IEnumerable<MemberGuildDTO>>, string>
    //{
    //    public async Task<IHttpResult<IEnumerable<MemberGuildDTO>>> ExecuteAsync(string discordUserId, CancellationToken aCancellationToken = default)
    //    {
    //        //get all member under the user id and then from each member.guildID get the guild and build the member guild dto
    //        var userMembershipList = await memberRepository.GetByDiscordUserIdAsync
    //        var exsitingGuildResult = await guildRepository.GetByIdAsync(ulong.Parse(discordUserId), aCancellationToken);
    //        if (!exsitingGuildResult.IsSuccess)
    //            return await guildRepository.AddAsync(new Guild(guildDTO.Id, guildDTO.Name, guildDTO.IconUrl), aCancellationToken)
    //                .Map(guild => guild.ToDto());
    //        if (exsitingGuildResult.IsSuccess && exsitingGuildResult.Value != null)
    //            return exsitingGuildResult.Map(guild => guild.ToDto());
    //        return Result.Failure<GuildDTO>(ApplicationErrors.Guilds.NotAdded);
    //    }
    //}
}
