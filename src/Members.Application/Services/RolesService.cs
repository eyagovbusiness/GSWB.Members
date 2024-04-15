using Common.Application.DTOs.Discord;
using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Application.Services
{
    public class RolesService : IRolesService
    {
        private readonly IRoleRepository _roleRepository;

        public RolesService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        #region IRoleRepository
        public async Task<IHttpResult<PaginatedRoleListDTO>> GetRoleList(
            int aPage, int aPageSize,
            string aSortBy,
            string? aNameFilter, RoleTypesEnum? aTypeFilter, PermissionsEnum? aPermissionsFilter,
            CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResult(aCancellationToken)
            .Bind(_ => _roleRepository.GetRoleListAsync(aPage, aPageSize, aSortBy, aNameFilter, aTypeFilter, aPermissionsFilter, aCancellationToken))
            .Bind(roleList => GetPaginatedRoleListDTO(roleList, aPage, aPageSize));

        public async Task<IHttpResult<IEnumerable<RoleDTO>>> UpdateRoleList(IEnumerable<RoleUpdateDTO> aRoleToUpdateList, CancellationToken aCancellationToken = default)
        => await _roleRepository.UpdateAsync(aRoleToUpdateList, aCancellationToken)
            .Map(roleUpdatedList => roleUpdatedList.Select(role => role.ToDto()));

        public async Task<IHttpResult<int>> GetRolesCount(CancellationToken aCancellationToken = default)
            => await _roleRepository.GetCountAsync();

        #region Discord interop
        public async Task<IHttpResult<RoleDTO>> AddRoleAsync(DiscordRoleDTO aNewDiscordRoleDTO, CancellationToken aCancellationToken = default)
        => await _roleRepository.AddAsync(aNewDiscordRoleDTO, aCancellationToken)
            .Map(newRole => newRole.ToDto());

        public async Task<IHttpResult<RoleDTO>> UpdateRoleAsync(DiscordRoleDTO aUpdatedDiscordRoleDTO, CancellationToken aCancellationToken = default)
         => await _roleRepository.UpdateAsync(aUpdatedDiscordRoleDTO, aCancellationToken)
            .Map(newRole => newRole.ToDto());

        public async Task<IHttpResult<RoleDTO>> DeleteRoleAsync(ulong aDiscordRoleIdDeleted, CancellationToken aCancellationToken = default)
        => await _roleRepository.DeleteAsync(aDiscordRoleIdDeleted, aCancellationToken)
            .Map(newRole => newRole.ToDto());

        #endregion

        #region Private
        private async Task<IHttpResult<PaginatedRoleListDTO>> GetPaginatedRoleListDTO(IEnumerable<Role> aRoleList, int aCurrentPage, int aPageSize)
        => await _roleRepository.GetCountAsync()
            .Map(memberCount => new PaginatedRoleListDTO(aCurrentPage, (int)Math.Ceiling((double)memberCount / aPageSize), aPageSize, memberCount, aRoleList.Select(role => role.ToDto()).ToArray()));

        #endregion

        #endregion

    }

}
