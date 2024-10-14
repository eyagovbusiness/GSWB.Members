using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Domain.Entities
{
    public partial class Guild
    {
        public virtual ICollection<Role> Roles { get; protected set; } = [];
        public virtual ICollection<GuildBooster> Boosters { get; protected set; } = [];

        public IHttpResult<Guild> AddRoles(IEnumerable<Role> roles)
        {
            roles.ToList().ForEach(Roles.Add);
            return Result.SuccessHttp(this);
        }

        public IHttpResult<Guild> AddBoost(GuildBooster booster)
        {
            Boosters.Add(booster);
            return Result.SuccessHttp(this);
        }


    }
}
