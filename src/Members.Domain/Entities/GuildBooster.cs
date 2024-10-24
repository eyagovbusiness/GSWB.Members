using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    public class GuildBooster: Entity<Guid>
    {
        public required ulong UserId { get; set; }
        public required Guild Guild { get; set; }

    }
}
