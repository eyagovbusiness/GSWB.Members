using Ardalis.Specification;
using Common.Application.DTOs.Members;
using Members.Application.Mapping;
using Members.Application.Specifications;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using TGF.CA.Application.Contracts.Services;
using TGF.CA.Application.DTOs;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members.Read
{
    /// <summary>
    /// Use case to list Members
    /// </summary>
    public class GetMembersPage(IMemberRepository memberRepository, IRoleQueryRepository roleQueryRepository, IPagedListMapperService pagedListMapperService)
        : IUseCase<IHttpResult<PagedListDTO<MemberDetailDTO>>, MemberPageSpecification>
    {
        public async Task<IHttpResult<PagedListDTO<MemberDetailDTO>>> ExecuteAsync(MemberPageSpecification request, CancellationToken cancellationToken = default)
        {
            IEnumerable<Role> guildRoleList = [];
            IEnumerable<MemberDetailDTO> memberDTOList = [];
            return await roleQueryRepository.GetListAsync(new RolesOfGuildIdSpec(ulong.Parse(request._guildId)), cancellationToken)
            .Tap(guildRoles => guildRoleList = guildRoles)
            .Bind(_ => memberRepository.GetListAsync(request, cancellationToken))
            .Tap(members => memberDTOList = members
                .Select(member => member.ToDetailDto(
                    guildRoleList
                    .Where(role =>
                        member.Roles
                        .Any(memberRole => memberRole.RoleId == role.RoleId)
                    )
                ))
            )
            .Bind(_ => memberRepository.GetCountAsync(cancellationToken))
            .Map(membersCount => pagedListMapperService.ToPagedListDTO(memberDTOList, request, membersCount));
        }
    }
}
