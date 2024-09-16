using Common.Application.DTOs.Roles;
using Members.Domain.Entities;

namespace Members.Application.Mapping
{
    public static class RoleMapping
    {
        public static RoleDTO ToDto(this Role aRole)
            => new(aRole.Name, aRole.Permissions, aRole.RoleType, aRole.Description, aRole.Id.ToString(), aRole.Position);
    }
}
