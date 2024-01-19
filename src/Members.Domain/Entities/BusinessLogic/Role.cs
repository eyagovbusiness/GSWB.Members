using Common.Domain.ValueObjects;

namespace Members.Domain.Entities
{
    public partial class Role
    {
        public bool IsApplicationRole()
            => RoleType == RoleTypesEnum.ApplicationRole;
        public bool IsDiscordOnly()
            => RoleType == RoleTypesEnum.DiscordOnly;
        public bool IsLicense()
            => RoleType == RoleTypesEnum.License;

        #region Private

        private PermissionsEnum SetPermissions(PermissionsEnum aNewPermissions)
        {
            if (aNewPermissions != PermissionsEnum.None && RoleType != RoleTypesEnum.ApplicationRole)
                throw new InvalidOperationException($"Cannot set permission for a role which RoleType is not {nameof(RoleTypesEnum.ApplicationRole)}");
            return aNewPermissions;
        }

        private RoleTypesEnum SetRoleType(RoleTypesEnum aNewRoleType)
        {
            if (RoleType == RoleTypesEnum.ApplicationRole)
                Permissions = default;
            return aNewRoleType;
        }

        #endregion

    }
}
