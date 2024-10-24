using Members.Domain.Entities;

namespace Members.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject with information about the result of a member roles update, with the cupdated member after the roles update and a boolean value to determine if the roles updated modified the permissions fo the member.
    /// </summary>
    /// <param name="Member">The member after the roles update.</param>
    /// <param name="IsPermissionsChanged">Boolean value to determine if the roles updated modified the permissions fo the member.</param>
    public record MemberRolesUpdateResult(Member Member, bool IsPermissionsChanged);
}
