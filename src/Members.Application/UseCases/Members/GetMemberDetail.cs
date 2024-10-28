﻿using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Members
{
    /// <summary>
    /// Use case to get a <see cref="MemberDetailDTO"/> (which includes the role dtos, and additional member properties not included in the basic DTO
    /// </summary>
    public class GetMemberDetail(
        IMemberRepository memberRepository,
        IRoleQueryRepository roleQueryRepository
    )
         : IUseCase<IHttpResult<MemberDetailDTO>, GuildAndUserId>
    {
        public Task<IHttpResult<MemberDetailDTO>> ExecuteAsync(GuildAndUserId request, CancellationToken cancellationToken = default)
        {
            Member lMember = default!;
            return memberRepository.GetByGuildAndUserIdsAsync(request.GuildId, request.UserId, cancellationToken)
                .Tap(member => lMember = member)
                .Bind(member => roleQueryRepository.GetByIdListAsync(member.Roles.Select(memberRoles => memberRoles.RoleId)))
                .Map(roles => lMember.ToDetailDto(roles));
        }
    }
}
