using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using Members.Domain.Validation.Guild;
using Members.Domain.ValueObjects;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Domain.Services
{
    public class GuildMemberRoleService(GuildRoleIdValidator guildRoleIdValidator)
    {
        /// <summary>
        /// Performs a role revocation for the member in the guild of the given roles.
        /// </summary>
        /// <param name="guild">The guild aggregate</param>
        /// <param name="member">The member aggregate</param>
        /// <param name="revokeRoleIdList">The list of roleId to be revoked for the member.</param>
        /// <returns>Result of <see cref="MemberRolesUpdateResult"/> with the updated member after the roles revocation and boolean declaring if the permissions were updated after the the action or not.</returns>
        public IHttpResult<MemberRolesUpdateResult> RevokeRoles(Guild guild, Member member, IEnumerable<ulong> revokeRoleIdList)
        {
            PermissionsEnum permissionsBefore = default!;
            return GetPermissions(guild, member)
                .Tap(permissions => permissionsBefore = permissions)
                .Bind(_ => member.RevokeRoles(revokeRoleIdList))
                .Bind(member => GetPermissions(guild, member))
                .Map(permissions => new MemberRolesUpdateResult(member, permissions != permissionsBefore));
        }

        /// <summary>
        /// Check if assigning the given roles to the member in the guild would modify the permissions of the member.
        /// </summary>
        /// <param name="guild">The guild aggregate</param>
        /// <param name="member">The member aggregate</param>
        /// <param name="addRoleIdList">The list of roleId to be assigned to the member.</param>
        /// <returns>Result of <see cref="MemberRolesUpdateResult"/> with the updated member after assigning the role and a boolean declaring if the permissions were updated after the the action or not.</returns>
        public async Task<IHttpResult<MemberRolesUpdateResult>> AssignRoles(Guild guild, Member member, IEnumerable<ulong> addRoleIdList)
        {
            PermissionsEnum permissionsBefore = default!;
            return await GetPermissions(guild, member)
                .Tap(permissions => permissionsBefore = permissions)
                .Bind(_ => member.AssignRoles(addRoleIdList, guildRoleIdValidator))
                .Bind(member => GetPermissions(guild, member))
                .Map(permissions => new MemberRolesUpdateResult(member, permissions != permissionsBefore));
        }

        /// <summary>
        /// Get this member's permissions, calculated by Bitwise OR to accumulate permissions from each permissions of all roles assigned to the member.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static
        public IHttpResult<PermissionsEnum> GetPermissions(Guild guild, Member member)
#pragma warning restore CA1822 // Mark members as static
        {
            // Step 1: Extract member's role IDs into a HashSet for fast lookups
            var memberRoleIds = new HashSet<ulong>(member.Roles.Select(memberRole => memberRole.RoleId));

            // Step 2: Filter guild roles that are application roles and are assigned to the member
            var memberApplicationRoles = guild.Roles
                .Where(role => role.IsApplicationRole() && memberRoleIds.Contains(role.Id));

            // Step 3: Accumulate permissions using bitwise OR
            PermissionsEnum totalPermissions = PermissionsEnum.None;
            foreach (var role in memberApplicationRoles)
                totalPermissions |= role.Permissions;

            // Step 4: Return the accumulated permissions
            return Result.SuccessHttp(totalPermissions);
        }
    }
}
