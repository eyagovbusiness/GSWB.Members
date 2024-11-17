using Ardalis.Specification;
using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Application.Specifications;
using Members.Application.Specifications.With;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use case to list Members from a list of ids
    /// </summary>
    public class ListMembersByIds(IMemberRepository memberRepository, IRoleQueryRepository roleQueryRepository)
        : IUseCase<IHttpResult<IEnumerable<MemberDetailDTO>>, IEnumerable<MemberKey>>
    {
        public async Task<IHttpResult<IEnumerable<MemberDetailDTO>>> ExecuteAsync(IEnumerable<MemberKey> request, CancellationToken cancellationToken = default)
        {
            IEnumerable<Role> guildRoleList = [];
            IEnumerable<MemberDetailDTO> memberDTOList = [];
            return await roleQueryRepository.GetListAsync(new RolesOfGuildIdSpec(request.First().GuildId), cancellationToken)
            .Tap(guildRoles => guildRoleList = guildRoles)
            .Bind(_ => memberRepository.GetByIdListAsync(request, new MemberWithRolesSpec(), cancellationToken))
            .Map(members => members
                .Select(member => member.ToDetailDto(
                    guildRoleList
                    .Where(role =>
                        member.Roles
                        .Any(memberRole => memberRole.RoleId == role.RoleId)
                    )
                ))
            );
        }
    }
}
