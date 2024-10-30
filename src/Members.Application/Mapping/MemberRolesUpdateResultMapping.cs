using Common.Application.DTOs.Members;
using Members.Domain.Entities;
using Members.Domain.ValueObjects;


namespace Members.Application.Mapping
{
    public static class MemberRolesUpdateResultMapping
    {
        public static MemberRolesUpdateResultDTO ToDto(this MemberRolesUpdateResult memberRolesUpdateResult, IEnumerable<Role> roles, bool aIncludeDiscordOnlyRoles = false)
        => new(memberRolesUpdateResult.Member.ToDetailDto(roles, aIncludeDiscordOnlyRoles), memberRolesUpdateResult.IsPermissionsChanged);
    }
}
