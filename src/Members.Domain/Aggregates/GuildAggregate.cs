using Common.Domain.ValueObjects;
using Members.Domain.Errors;
using Members.Domain.Validation;
using Members.Domain.ValueObjects.Role;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Members.Domain.Entities
{
    public partial class Guild
    {
        public virtual ICollection<Role> Roles { get; protected set; } = [];
        public virtual ICollection<GuildBooster> Boosters { get; protected set; } = [];

        public IHttpResult<Guild> AddRoles(IEnumerable<DiscordRoleValues> rolesValueList)
        {
            foreach (var roleValues in rolesValueList)
            {
                Role newRole = new()
                {
                    Guild = this,
                    Name = roleValues.Name,
                    Position = roleValues.Position,
                    RoleType = roleValues.Name == InvariantConstants.Role_Name_IsGuildSwarmAdmin ? RoleTypesEnum.ApplicationRole : RoleTypesEnum.DiscordOnly,
                    Permissions = roleValues.Name == InvariantConstants.Role_Name_IsGuildSwarmAdmin ? PermissionsEnum.Admin : PermissionsEnum.None
                };
                newRole.SetId(roleValues.Id);
                Roles.Add(newRole);
            }
                

            return Result.SuccessHttp(this);
        }

        public IHttpResult<Guild> UpdateRoles(IEnumerable<RoleUpdate> roleUpdates)
        {
            var roleUpdateDict = roleUpdates.ToDictionary(r => r.Id, r => r);
            var lDbRoleList = Roles
                .Where(r => roleUpdateDict.ContainsKey(r.Id));

            if (Roles.Count != roleUpdateDict.Count)
                return Result.Failure<Guild>(DomainErrors.Role.NotFoundIdList);

            lDbRoleList.ForEach(role =>
            {
                var dto = roleUpdateDict[role.Id];
                role.RoleType = dto.Type;
                role.Permissions = dto.Permissions;
                role.Description = dto.Description;
            });
            return Result.SuccessHttp(this);
        }

        public IHttpResult<Guild> UpdateRoles(IEnumerable<DiscordRoleValues> roleDiscordUpdates)
        {
            var roleUpdateDict = roleDiscordUpdates.ToDictionary(r => r.Id, r => r);
            var lDbRoleList = Roles
                .Where(r => roleUpdateDict.ContainsKey(r.Id));

            if (Roles.Count != roleUpdateDict.Count)
                return Result.Failure<Guild>(DomainErrors.Role.NotFoundIdList);

            lDbRoleList.ForEach(role =>
            {
                var dto = roleUpdateDict[role.Id];
                role.Position = dto.Position;
                role.Name = dto.Name;
            });
            return Result.SuccessHttp(this);
        }

        public IHttpResult<Guild> DeleteRoles(IEnumerable<ulong> roleIdList)
        {
            var rolesToDelete = Roles.Where(r => roleIdList.Contains(r.Id)).ToList();

            if (rolesToDelete.Count != roleIdList.Count())
                return Result.Failure<Guild>(DomainErrors.Role.NotFoundIdList);

            foreach (var role in rolesToDelete)
                Roles.Remove(role);

            return Result.SuccessHttp(this);
        }

        public IHttpResult<Guild> AddBoost(ulong userId)
        {
            Boosters.Add(new() { Guild = this, UserId = userId });
            return Result.SuccessHttp(this);
        }


    }
}
