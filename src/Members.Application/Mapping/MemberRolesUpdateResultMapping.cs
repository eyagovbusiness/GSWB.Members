using Common.Application.DTOs.Members;
using Members.Domain.ValueObjects;


namespace Members.Application.Mapping
{
    public static class MemberRolesUpdateResultMapping
    {
        public static MemberRolesUpdateResultDTO ToDto(this MemberRolesUpdateResult memberRolesUpdateResult)
        => new(memberRolesUpdateResult.Member.ToDetailDto(), memberRolesUpdateResult.IsPermissionsChanged);
    }
}
