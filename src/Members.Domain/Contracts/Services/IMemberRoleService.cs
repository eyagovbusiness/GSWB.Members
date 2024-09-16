using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;

namespace Members.Domain.Contracts.Services
{
    public interface IMemberRoleService
    {
        Task<IHttpResult<PermissionsEnum>> CalculatePermissions(Guid memberId, CancellationToken aCancellationToken);
        Task<IHttpResult<Role?>> GetHighestRole(Guid aMemberId, CancellationToken aCancellationToken);
    }
}