using Members.Domain.Entities;

namespace Members.Domain.ValueObjects
{
    public record MemberRolesUpdateResult(Member Member, bool IsPermissionsChanged);
}
