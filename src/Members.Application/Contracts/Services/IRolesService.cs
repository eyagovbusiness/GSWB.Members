using Common.Application.DTOs;
using Common.Application.DTOs.Discord;
using Common.Domain.ValueObjects;
using TGF.Common.ROP.HttpResult;

namespace Members.Application
{
    public interface IRolesService
    {
        /// <summary>
        /// Get the whole list of Role entities.
        /// </summary>
        /// <returns>The list with all the Role entities as DTO.</returns>
        public Task<IHttpResult<PaginatedRoleListDTO>> GetRoleList(
            int aPage, int aPageSize,
            string aSortBy,
            string? aNameFilter, RoleTypesEnum? aTypeFilter, PermissionsEnum? aPermissionsFilter,
            CancellationToken aCancellationToken = default);

        /// <summary>
        /// Updates a list of Role entities with user changes from the web application.
        /// </summary>
        /// <param name="aRoleToUpdateList">List of <see cref="RoleUpdateDTO"/> with the updated field values of each role.</param>
        /// <returns>The list of updated Roles after the updates.</returns>
        public Task<IHttpResult<IEnumerable<RoleDTO>>> UpdateRoleList(IEnumerable<RoleUpdateDTO> aRoleToUpdateList, CancellationToken aCancellationToken = default);

        #region Discord interop
        /// <summary>
        /// Adds a new Role entitiy from a <see cref="DiscordRoleDTO"/> with onformation about a new role created in Discord.
        /// </summary>
        /// <param name="aNewDiscordRoleDTO">DTO with information of the new Role created in Discord.</param>
        /// <returns>DTO of the new Role entity created.</returns>
        /// <remarks>Interop with discord dedicated method.</remarks>
        public Task<IHttpResult<RoleDTO>> AddRoleAsync(DiscordRoleDTO aNewDiscordRoleDTO, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Updates a Role entitiy related with the given <see cref="DiscordRoleDTO"/> with onformation about an update of the role that happened in Discord.
        /// </summary>
        /// <param name="aUpdatedDiscordRoleDTO">DTO with information of the a Role that was updated in Discord.</param>
        /// <returns>DTO of the new Role entity updated.</returns>
        /// <remarks>Interop with discord dedicated method.</remarks>
        public Task<IHttpResult<RoleDTO>> UpdateRoleAsync(DiscordRoleDTO aUpdatedDiscordRoleDTO, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Deletes a Role entitiy related with the given <see cref="DiscordRoleDTO"/> which was deleted in Discord.
        /// </summary>
        /// <param name="aDiscordRoleIdDeleted">DTO with information of a Role that was deleted in Discord.</param>
        /// <returns>DTO of the new Role entity deleted.</returns>
        /// <remarks>Interop with discord dedicated method.</remarks>
        public Task<IHttpResult<RoleDTO>> DeleteRoleAsync(ulong aDiscordRoleIdDeleted, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the total count of guild roles.
        /// </summary>
        public Task<IHttpResult<int>> GetRolesCount(CancellationToken aCancellationToken = default);

        #endregion

    }
}
