using Common.Application;

namespace Members.Application
{
    /// <summary>
    /// Supports role-related operations more only related to the infrastructure layer. The list of roles come from Discord, at application level they can be only updated and modified certain properties.
    /// Despite this service to be an infrastructure service, the interface can be used at the Application level to trigger the roles update while keeping protection against direct role create/delete operations which are not an application concern.
    /// </summary>
    /// <remarks>Depends on <see cref="IRoleRepository"/> and <see cref="ISwarmBotCommunicationService"/>.</remarks>
    public interface IRolesInfrastructureService
    {
        /// <summary>
        /// Synchronizes the roles in the database with roles fetched from Discord.
        /// </summary>
        /// <returns>True if the synchronization was successful.</returns>
        public Task<bool> SyncRolesWithDiscordAsync(ulong guildId, CancellationToken aCancellationToken = default);
    }
}
