using Common.Application;
using Common.Application.DTOs.Discord;
using Common.Domain.ValueObjects;
using Members.Application;
using Members.Domain.Entities;
using Members.Infrastructure.Repositories;

namespace Members.Infrastructure.Services
{

    internal class RolesInfrastructureService : IRolesInfrastructureService
    {
        private readonly RoleRepository _roleRepository;
        private readonly ISwarmBotCommunicationService _SwarmBotCommunicationService;

        public RolesInfrastructureService(IRoleRepository aRoleRepository, ISwarmBotCommunicationService aSwarmBotCommunicationService)
        {
            _roleRepository = (aRoleRepository as RoleRepository)!;
            _SwarmBotCommunicationService = aSwarmBotCommunicationService;
        }

        #region IRolesInfrastructureService

        public async Task<bool> SyncRolesWithDiscordAsync(CancellationToken aCancellationToken)
        {
            var GetDiscordRoleListResult = await _SwarmBotCommunicationService.GetDiscordRoleList(aCancellationToken);
            if (GetDiscordRoleListResult == null || !GetDiscordRoleListResult.IsSuccess)
                throw new Exception("Error during the initial Roles sync with Discord.");

            var lGuildServerDiscordRoleList = GetDiscordRoleListResult.Value;
            var lDatabaseRoleList = await _roleRepository.GetAll(aCancellationToken);


            if (aCancellationToken.IsCancellationRequested || !lDatabaseRoleList.IsSuccess)
                return false;

            var lDiscordRoleIds = new HashSet<ulong>(lGuildServerDiscordRoleList.Select(r => ulong.Parse(r.Id)));
            var lDatabaseRoleIds = new HashSet<ulong>(lDatabaseRoleList.Value.Select(r => r.DiscordRoleId));

            await AddNewRolesToDatabase(lGuildServerDiscordRoleList, lDatabaseRoleIds);
            await DeleteRolesFromDatabase(lDatabaseRoleList.Value, lDiscordRoleIds);
            await UpdateExistingRoles(lGuildServerDiscordRoleList, lDatabaseRoleList.Value);

            return true;
        }

        #endregion

        #region Private 

        /// <summary>
        /// Adds new roles fetched from Discord to the database.
        /// </summary>
        /// <param name="aDiscordRoleList">List of roles from Discord.</param>
        /// <param name="aExistingRoleIdList">List of role IDs that already exist in the database.</param>
        private async Task AddNewRolesToDatabase(IEnumerable<DiscordRoleDTO> aDiscordRoleList, HashSet<ulong> aExistingRoleIdList)
        {
            var lRolesToAddList = aDiscordRoleList
                .Where(role => !aExistingRoleIdList.Contains(ulong.Parse(role.Id)))
                .Select(role => new Role
                {
                    DiscordRoleId = ulong.Parse(role.Id),
                    Name = role.Name,
                    Position = role.Position,
                    RoleType = role.Name == "Admin" ? RoleTypesEnum.ApplicationRole : RoleTypesEnum.DiscordOnly,
                    Permissions = role.Name == "Admin" ? PermissionsEnum.Admin : PermissionsEnum.None
                })
                .ToArray();

            if (lRolesToAddList.Length > 0)
                _ = await _roleRepository.AddRoleListAsync(lRolesToAddList);
        }

        /// <summary>
        /// Deletes roles from the database that do not exist in the fetched Discord roles.
        /// </summary>
        /// <param name="aDatabaseRoleList">List of roles from the database.</param>
        /// <param name="aDiscordRoleIdList">List of role IDs from Discord.</param>
        private async Task DeleteRolesFromDatabase(IEnumerable<Role> aDatabaseRoleList, HashSet<ulong> aDiscordRoleIdList)
        {
            var lRolesToDeleteList = aDatabaseRoleList
                .Where(role => !aDiscordRoleIdList.Contains(role.DiscordRoleId))
                .ToArray();

            if (lRolesToDeleteList.Length > 0)
                await _roleRepository.DeleteRoleListAsync(lRolesToDeleteList);
        }

        /// <summary>
        /// Updates the roles in the database to match the roles fetched from Discord.
        /// </summary>
        /// <param name="aDiscordRoleList">List of roles from Discord.</param>
        /// <param name="aDatabaseRoleList">List of roles from the database.</param>
        private async Task UpdateExistingRoles(IEnumerable<DiscordRoleDTO> aDiscordRoleList, IEnumerable<Role> aDatabaseRoleList)
        {
            var lRolesToUpdateList = new List<Role>();

            foreach (var lDbRole in aDatabaseRoleList)
            {
                var discordRole = aDiscordRoleList.FirstOrDefault(role => ulong.Parse(role.Id) == lDbRole.DiscordRoleId);
                if (discordRole != null && (discordRole.Name != lDbRole.Name || discordRole.Position != lDbRole.Position))
                {
                    lDbRole.Name = discordRole.Name;
                    lDbRole.Position = discordRole.Position;
                    lRolesToUpdateList.Add(lDbRole);
                }
            }

            if (lRolesToUpdateList.Count > 0)
                _ = await _roleRepository.UpdateRoleListAsync(lRolesToUpdateList);
        }

        #endregion

    }
}
