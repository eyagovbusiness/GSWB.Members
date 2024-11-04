using Ardalis.Specification;
using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using TGF.CA.Application.Contracts.Services;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use case to list Members
    /// </summary>
    //public class ListMembersByIds(IMemberRepository memberRepository, IPagedListMapperService pagedListMapperService)
    //    : IUseCase<IHttpResult<IEnumerable<MemberDTO>>, IEnumerable<Guid>>
    //{
    //    public async Task<IHttpResult<IEnumerable<MemberDTO>>> ExecuteAsync(IEnumerable<GuildAndUserId> request, CancellationToken cancellationToken = default)
    //    {
    //        IEnumerable<MemberDTO> memberDTOList = [];
    //        return await memberRepository.GetByGuildAndUserIdsAsync(request, cancellationToken)
    //        .Tap(members => memberDTOList = members.Select(role => role.ToDto()))
    //        .Map(members => members.Select(member => member.ToDetailDto());
    //    }
    //}
}
