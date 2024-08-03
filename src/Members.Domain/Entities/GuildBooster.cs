using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    public class GuildBooster: Entity<Guid>
    {
        public required Guid MemberId { get; set; }
        public required Guild Guild { get; set; }

    }
}
