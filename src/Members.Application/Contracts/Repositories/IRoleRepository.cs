using Common.Application.DTOs;
using Common.Application.DTOs.Discord;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;

namespace Members.Application
{
    /// <summary>
    /// Represents a repository for application Roles with the allowed operations.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Asynchronously retrieves a list of roles by the associated DiscordRoleId.
        /// </summary>
        /// <param name="aDiscordRoleIdList">List with the DiscordRoleId.</param>
        /// <returns>List with all the Roles associated with any of the DiscordRoleIds provided.</returns>
        public Task<IHttpResult<IEnumerable<Role>>> GetListByDiscordRoleId(IEnumerable<ulong> aDiscordRoleIdList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves all the application roles registered in the database.
        /// </summary>
        /// <returns>List of all the guild roles.</returns>
        public Task<IHttpResult<IEnumerable<Role>>> GetRoleListAsync(
            int aPage, int aPageSize,
            string aSortBy,
            string? aNameFilter = null, RoleTypesEnum? aTypeFilter = null, PermissionsEnum? aPermissionsFilter = null,
            CancellationToken aCancellationToken = default);

        /// <summary>
        /// Asynchronously updates the specified list of roles.
        /// </summary>
        /// <param name="aRoleToUpdateList">List of <see cref="RoleUpdateDTO"/> with the role changes to be updated in DB.</param>
        /// <returns>List of roles updated.</returns>
        /// <remarks>Only Permissions, Description, and RoleType properties can be updated manually.</remarks>
        public Task<IHttpResult<IEnumerable<Role>>> UpdateAsync(IEnumerable<RoleUpdateDTO> aRoleToUpdateList, CancellationToken aCancellationToken = default);

        #region Discord interop
        /// <summary>
        /// Asynchronously adds a new Role from a given <see cref="DiscordRoleDTO"/> from Discord.
        /// </summary>
        /// <param name="aNewDiscordRoleDTO">DiscordRoleDTO with the needed data comming from Discord to create the respective Role entitiy.</param>
        /// <returns>The new Role entitiy.</returns>
        /// <remarks>Interop with discord dedicated method.</remarks>
        public Task<IHttpResult<Role>> AddAsync(DiscordRoleDTO aNewDiscordRoleDTO, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Asynchronously updates the given role.
        /// </summary>
        /// <param name="aUpdatedDiscordRoleDTO">DiscordRoleDTO with the needed data to update changes comming from Discord in the respective Role entitiy.</param>
        /// <returns>The updated Role entitiy.</returns>
        /// <remarks>Interop with discord dedicated method.</remarks>
        public Task<IHttpResult<Role>> UpdateAsync(DiscordRoleDTO aUpdatedDiscordRoleDTO, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Asynchronously deleted a Role entitiy from its DiscordRoleId which was deleted from Discord.
        /// </summary>
        /// <param name="aDiscordRoleIdDeleted">DiscordRole Id which was deleted in Discord..</param>
        /// <returns>The deleted Role entitiy.</returns>
        /// <remarks>Interop with discord dedicated method.</remarks>
        public Task<IHttpResult<Role>> DeleteAsync(ulong aDiscordRoleIdDeleted, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the number of registered guild roles.
        /// </summary>
        /// <returns>Returns the number registered guild roles or Error.</returns>
        public Task<IHttpResult<int>> GetCountAsync(CancellationToken aCancellationToken = default);

        #endregion

    }
}
