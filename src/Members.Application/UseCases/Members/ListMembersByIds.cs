using Ardalis.Specification;
using Common.Application.DTOs.Members;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using TGF.CA.Application.Contracts.Services;
using TGF.CA.Application.DTOs;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use case to list Members
    /// </summary>
    public class ListMembers(IMemberRepository memberRepository, IPagedListMapperService pagedListMapperService)
        : IUseCase<IHttpResult<PagedListDTO<MemberDTO>>, ISpecification<Member>>
    {
        public async Task<IHttpResult<PagedListDTO<MemberDTO>>> ExecuteAsync(ISpecification<Member> request, CancellationToken cancellationToken = default)
        {
            IEnumerable<MemberDTO> memberDTOList = [];
            return await memberRepository.GetListAsync(request, cancellationToken)
            .Tap(members => memberDTOList = members.Select(role => role.ToDto()))
            .Bind(_ => memberRepository.GetCountAsync(cancellationToken))
            .Map(membersCount => pagedListMapperService.ToPagedListDTO(memberDTOList, request, membersCount));
        }
    }
}
