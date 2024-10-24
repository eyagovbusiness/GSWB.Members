using Common.Domain.ValueObjects;

namespace Members.Domain.ValueObjects.Role
{
    /// <summary>
    /// ValueObject grouping the values that can be updated in a role update operation and the role Id.
    /// </summary>
    /// <param name="Permissions">The role permissions.</param>
    /// <param name="Type">The type of role it is.</param>
    /// <param name="Description">The description of the role.</param>
    /// <param name="Id">The Id of the role(just to identify to which role belongsa this values in order to be updated).</param>
    public record RoleUpdate(PermissionsEnum Permissions, RoleTypesEnum Type, string? Description, ulong Id);
}
