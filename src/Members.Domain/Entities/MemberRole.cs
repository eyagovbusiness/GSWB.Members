using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    public class MemberRole : Entity<Guid>
    {
        public required Member Member { get; set; }
        public required ulong RoleId {  get; set; } 
    }
}
