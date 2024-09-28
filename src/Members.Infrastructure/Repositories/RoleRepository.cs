using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Members.Application;
using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB.Repository;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Infrastructure.Repositories
{
    internal class RoleRepository(MembersDbContext aContext, ILogger<RoleRepository> aLogger)
        : RepositoryBase<RoleRepository, MembersDbContext, Role, ulong>(aContext, aLogger), IRoleRepository, ISortRepository
    {

        #region IRoleRepository
        public async Task<IHttpResult<IEnumerable<Role>>> GetRoleListAsync(
            int aPage, int aPageSize,
            string aSortBy,
            string? aNameFilter = null, RoleTypesEnum? aTypeFilter = null, PermissionsEnum? aPermissionsFilter = null,
            CancellationToken aCancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken) =>
        {
            var lQuery = _context.Roles
            .AsQueryable();

            lQuery = ApplyFilters(lQuery, aNameFilter, aTypeFilter, aPermissionsFilter);
            lQuery = ISortRepository.ApplySorting(lQuery, aSortBy, ISortRepository.SortDirection.Descending);

            return await lQuery
                .Skip((aPage - 1) * aPageSize)
                .Take(aPageSize)
                .ToListAsync(aCancellationToken) as IEnumerable<Role>;
        }, aCancellationToken);

        public async Task<IHttpResult<IEnumerable<Role>>> GetListByDiscordRoleId(IEnumerable<ulong> aDiscordRoleIdList, CancellationToken aCancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken)
            => await _context.Roles
                .Where(role => aDiscordRoleIdList.Contains(role.Id))
                .OrderByDescending(r => r.Position)
                .ToListAsync(aCancellationToken) as IEnumerable<Role>
        , aCancellationToken);

        public async Task<IHttpResult<IEnumerable<Role>>> UpdateAsync(IEnumerable<RoleUpdateDTO> aRoleToUpdateList, CancellationToken aCancellationToken = default)
        => await TryCommandAsync(async (aCancellationToken) =>
        {
            var lRolesDict = aRoleToUpdateList.ToDictionary(r => ulong.Parse(r.Id), r => r);
            var lDbRoleList = await _context.Roles
                .Where(r => lRolesDict.Keys.Contains(r.Id))
                .ToListAsync(aCancellationToken);

            if (lDbRoleList.Count != lRolesDict.Count)
                return Result.Failure<IEnumerable<Role>>(InfrastructureErrors.RolesDb.ListNotFoundId);

            lDbRoleList.ForEach(role =>
            {
                var dto = lRolesDict[role.Id];
                role.RoleType = dto.Type;
                role.Permissions = dto.Permissions;
                role.Description = dto.Description;
            });
            return Result.SuccessHttp(lDbRoleList as IEnumerable<Role>);

        }, aCancellationToken);

        #region Discord interop
        public async Task<IHttpResult<Role>> AddAsync(DiscordRoleDTO aNewDiscordRole, CancellationToken aCancellationToken = default)
        => await TryCommandAsync(async (aCancellationToken) =>
        {
            var lNewRole = new Role
            {
                Name = aNewDiscordRole.Name,
                Position = aNewDiscordRole.Position,
            };
            lNewRole.SetId(ulong.Parse(aNewDiscordRole.Id));
            await _context.Roles.AddAsync(lNewRole, aCancellationToken);
            return lNewRole;
        }, aCancellationToken);

        public async Task<IHttpResult<Role>> UpdateAsync(DiscordRoleDTO aUpdatedDiscordRoleDTO, CancellationToken aCancellationToken = default)
        => await TryCommandAsync(async (aCancellationToken) =>
        {
            var lDiscordRoleIdToFind = ulong.Parse(aUpdatedDiscordRoleDTO.Id);
            var lRoleToUpdate = await _context.Roles.FirstAsync(role => role.Id == lDiscordRoleIdToFind, aCancellationToken);
            _ = UpdateRole(lRoleToUpdate, aUpdatedDiscordRoleDTO);
            //_context.Roles.Update(lRoleToUpdate); update only needed to update entities not created by EF(no change tracker setup).In this case the entitiy was created by EF on reading DB.
            return lRoleToUpdate;
        }, aCancellationToken);

        public async Task<IHttpResult<Role>> DeleteAsync(ulong aDiscordRoleIdDeleted, CancellationToken aCancellationToken = default)
        => await TryCommandAsync(async (aCancellationToken) =>
        {
            var lRoleToDelete = await _context.Roles.FirstAsync(role => role.Id == aDiscordRoleIdDeleted);
            _context.Roles.Remove(lRoleToDelete);
            return lRoleToDelete;
        }, aCancellationToken);

        public async Task<IHttpResult<int>> GetCountAsync(CancellationToken aCancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken)
            => await _context.Roles.CountAsync(aCancellationToken)
        , aCancellationToken);

        #endregion

        #endregion

        #region Internal
        /// <summary>
        /// Used to get the list of all roles in the sync roles with discord in the RolesInfrastructureService.
        /// </summary>
        internal async Task<IHttpResult<IEnumerable<Role>>> GetAll(CancellationToken aCancellationToken = default)
        => await TryQueryAsync(async (aCancellationToken)
            => await _context.Roles
                .OrderByDescending(r => r.Position)
                .ToListAsync(aCancellationToken) as IEnumerable<Role>
        , aCancellationToken);

        /// <summary>
        /// Asynchronously adds a list of new roles to the database.
        /// </summary>
        /// <param name="aNewRoleList">The roles to be added.</param>
        /// <returns>List of added roles or Error.</returns>
        internal async Task<IHttpResult<IEnumerable<Role>>> AddRoleListAsync(IEnumerable<Role> aNewRoleList, CancellationToken aCancellationToken = default)
        => await TryCommandAsync(async (aCancellationToken) =>
        {
            await _context.Roles.AddRangeAsync(aNewRoleList, aCancellationToken);
            return aNewRoleList;
        }, aCancellationToken);

        /// <summary>
        /// Asynchronously updates a list of roles in the database.
        /// </summary>
        /// <param name="aRoleListToUpdate">The roles to be updated.</param>
        /// <returns>List of udpated roles or Error.</returns>
        internal async Task<IHttpResult<IEnumerable<Role>>> UpdateRoleListAsync(IEnumerable<Role> aRoleListToUpdate, CancellationToken aCancellationToken = default)
        => await TryCommandAsync(() =>
        {
            _context.Roles.UpdateRange(aRoleListToUpdate);
            return aRoleListToUpdate;
        }, aCancellationToken);


        /// <summary>
        /// Asynchronously deletes a list of roles from the database.
        /// </summary>
        /// <param name="aRoleListToDelete">The roles to be deleted.</param>
        /// <returns>List of deleted roles or Error</returns>
        internal async Task<IHttpResult<IEnumerable<Role>>> DeleteRoleListAsync(IEnumerable<Role> aRoleListToDelete, CancellationToken aCancellationToken = default)
        => await TryCommandAsync(() =>
        {
            _context.Roles.RemoveRange(aRoleListToDelete);
            return aRoleListToDelete;
        }, aCancellationToken);

        #endregion

        #region Private
        private bool UpdateRole(Role aSource, DiscordRoleDTO aUpdateSource)
        {
            var lAnyUpdate = false;
            if (aSource.Position != aUpdateSource.Position)
            {
                aSource.Position = aUpdateSource.Position;
                lAnyUpdate = true;
            }
            if (aSource.Name != aUpdateSource.Name)
            {
                aSource.Name = aUpdateSource.Name;
                lAnyUpdate = true;
            }
            return lAnyUpdate;
        }

        private static IQueryable<Role> ApplyFilters(
            IQueryable<Role> aQuery,
            string? aNameFilter = null, RoleTypesEnum? aTypeFilter = null, PermissionsEnum? aPermissionsFilter = null)
        {
            if (!string.IsNullOrEmpty(aNameFilter))
            {
                var lLowercaseDiscordNameFilter = $"%{aNameFilter.ToLowerInvariant()}%";
                aQuery = aQuery.Where(m => EF.Functions.Like(m.Name.ToLower(), lLowercaseDiscordNameFilter));
            }

            if (aTypeFilter.HasValue)
                aQuery = aQuery.Where(m => m.RoleType == aTypeFilter);

            if (aPermissionsFilter.HasValue)
                aQuery = aQuery.Where(m => (m.Permissions & aPermissionsFilter) == aPermissionsFilter);

            return aQuery;
        }

        #endregion
    }
}
