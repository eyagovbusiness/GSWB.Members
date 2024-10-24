namespace Members.Domain.ValueObjects.Role
{
    /// <summary>
    /// ValueObject with the minimal data from a discord role required to create a new Role entitiy.
    /// </summary>
    /// <param name="Id">Id of the role.</param>
    /// <param name="Name">Name of the role.</param>
    /// <param name="Position">Position of the role in the guild roles hierarchy.</param>
    public record DiscordRoleValues(ulong Id, string Name, byte Position);
}
