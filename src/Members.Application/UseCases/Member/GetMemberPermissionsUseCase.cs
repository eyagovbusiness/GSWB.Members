using Common.Domain.ValueObjects;
using Members.Domain.Services;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Member
{
    public class GetMemberPermissionsUseCase(MemberRoleService aMemberRoleService) 
        : IUseCase<IHttpResult<PermissionsEnum>, Guid>
    {
        public Task<IHttpResult<PermissionsEnum>> ExecuteAsync(Guid aMemberId, CancellationToken aCancellationToken = default)
        => aMemberRoleService.CalculatePermissions(aMemberId, aCancellationToken);
    }
}
