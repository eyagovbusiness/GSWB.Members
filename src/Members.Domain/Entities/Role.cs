using Common.Domain.ValueObjects;
using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    /// <summary>
    /// Represents an application role. Represents a DiscordRole but it has a set of application permissions attached. The Id of this entitiy has the exactly the same Id as the role it represents in Dicord(DiscordRoleId).
    /// </summary>
    public partial class Role : Entity<ulong>
    {
        /// <summary>
        /// Name of this Role.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Position of this Role in the roles hierarchy(position matters).
        /// </summary>
        public byte Position { get; set; }

        private RoleTypesEnum _roleType;
        /// <summary>
        /// Specifies the role type for this Role.
        /// </summary>
        /// <remarks>Uses <see cref="Role.SetRoleType(RoleTypesEnum)"/> in the setter.</remarks>
        public RoleTypesEnum RoleType { get => _roleType; set { _roleType = SetRoleType(value); } }

        /// <summary>
        /// The description of this Role.
        /// </summary>
        public string? Description { get; set; }

        private PermissionsEnum _permissions;
        /// <summary>
        /// The set of application permissions assigned to this Role. 
        /// </summary>
        /// <remarks>Uses <see cref="Role.SetPermissions(PermissionsEnum)"/> in the setter.</remarks>
        public PermissionsEnum Permissions { get => _permissions; set { _permissions = SetPermissions(value); } }

        /// <summary>
        /// The list of Member assigned with this Role.
        /// </summary>
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();

        //public Guild Guild { get; set; }

    }
}
