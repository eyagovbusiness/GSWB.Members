using Common.Domain.ValueObjects;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Contracts.Services;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;

namespace Members.Domain.Services
{
    /// <summary>
    /// Domain service with the business logic between the Member aggregate and the Role Aggregate
    /// </summary>
    /// <param name="aMemberRepository"></param>
    /// <param name="aRoleRepository"></param>
    public class MemberRoleService(IMemberRepository aMemberRepository, IRoleRepository aRoleRepository) 
        : IMemberRoleService
    {
        public async Task<IHttpResult<PermissionsEnum>> CalculatePermissions(Guid aMemberId, CancellationToken aCancellationToken)
        => await aMemberRepository.GetByIdAsync(aMemberId, aCancellationToken)
        .Bind(member => aRoleRepository.GetByIdListAsync(member.Roles.Select(memberRole => memberRole.RoleId), aCancellationToken))
        .Map(CalculatePermissions);

        /// <summary>
        /// Get this member's highest application role, which determines this member's permissions within the application.
        /// </summary>
        public async Task<IHttpResult<Role?>> GetHighestRole(Guid aMemberId, CancellationToken aCancellationToken)
        => await aMemberRepository.GetByIdAsync(aMemberId, aCancellationToken)
        .Bind(member => aRoleRepository.GetByIdListAsync(member.Roles.Select(memberRole => memberRole.RoleId), aCancellationToken))
        .Map(roles => roles.Where(r => r.IsApplicationRole()).MaxBy(role => role.Position));

        public async Task<IHttpResult<Role?>> AssignRoleList(Guid aMemberId, IEnumerable<Guid> aRoleIdList, CancellationToken aCancellationToken)
        => await this.GetHighestRole(aMemberId, aCancellationToken)
        .Bind(_ => _roleRepository.GetByIdListAsync(aAssignRoleList.Select(role => ulong.Parse(role.Id)).ToArray(), aCancellationToken))
        .Tap(roleToAssignList =>
        {
            aMember.AssignRoleList(roleToAssignList.Select(role => role.Id));
            lIsPermissionsUpdated = roleToAssignList.Any(role => role.IsApplicationRole() && role.Position > (lMemberHighestRole.Value?.Position ?? default));
        })
        .Bind(_ => _memberRepository.Update(aMember, aCancellationToken))
        .Map(_ => lIsPermissionsUpdated);

        #region Private
        // Get this member's permissions, calculated by Bitwise OR to accumulate permissions from each permissions of all roles assigned to the member.
        private static PermissionsEnum CalculatePermissions(IEnumerable<Role> aRoleList)
        {
            PermissionsEnum lTotalPermissions = PermissionsEnum.None;
            foreach (var lRole in aRoleList)
                lTotalPermissions |= lRole.Permissions;
            return lTotalPermissions;
        }

        #endregion

    }
}
