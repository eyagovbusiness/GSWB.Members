
namespace Members.Domain.ValueObjects.Role
{
    public class RoleKey(ulong roleId, ulong guildId)
    {
        public ulong RoleId { get; set; } = roleId;
        public ulong GuildId { get; set; } = guildId;

        // Override Equals and GetHashCode to allow proper comparisons in Contains
        public override bool Equals(object? obj)
        {
            if (obj is not RoleKey other)
                return false;

            return RoleId == other.RoleId && GuildId == other.GuildId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RoleId, GuildId);
        }
    }
}
