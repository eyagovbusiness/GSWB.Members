
namespace Members.Domain.Entities
{
    public partial class Guild
    {
        public virtual ICollection<Role> Roles { get; set; } = [];
        public virtual ICollection<GuildBooster> Boosters { get; set; } = [];

    }
}
