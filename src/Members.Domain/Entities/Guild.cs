using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    public class Guild : Entity<Guid>
    {
        public required string Name { get; set; }
        public required ulong DiscordGuildId { get; set; }
    }
}
