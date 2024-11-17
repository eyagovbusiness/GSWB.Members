using Common.Domain.ValueObjects;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using Members.Domain.Services;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members.Read
{
    /// <summary>
    /// Use case to get the calculated permissions of a given member from its Id.
    /// </summary>
    /// <returns>The permissions of the member.</returns>
    public class GetMemberPermissions(IMemberRepository memberRepository, IGuildRepository guildRepository, GuildMemberRoleService guildMemberRoleService) : IUseCase<IHttpResult<PermissionsEnum>, MemberKey>
    {
        public async Task<IHttpResult<PermissionsEnum>> ExecuteAsync(MemberKey request, CancellationToken cancellationToken = default)
        {
            Member lMember = default!;
            return await memberRepository.GetByIdAsync(request, cancellationToken)
                .Tap(member => lMember = member)
                .Bind(member => guildRepository.GetByIdAsync(member.GuildId, cancellationToken))
                .Bind(guild => guildMemberRoleService.GetPermissions(guild, lMember));
        }

    }
}
