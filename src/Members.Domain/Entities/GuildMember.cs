

namespace Members.Domain.Entities
{
    public class GuildMember
    {
        public required Member Member { get; set; }
        public required ulong GuildId { get; set; }
    }
}
