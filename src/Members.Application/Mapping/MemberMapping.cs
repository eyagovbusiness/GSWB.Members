using Common.Application.DTOs;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using System.Collections.Immutable;

namespace Members.Application.Mapping
{
    public static class MemberMapping
    {
        public static MemberDTO ToDto(this Member aMember)
        {
            var lRoleDTOList = aMember.Roles
                .Select(role => role.ToDto())
                .ToImmutableArray();
            return new MemberDTO(aMember.DiscordGuildDisplayName, aMember.GameHandle, aMember.Status, lRoleDTOList);
        }
        public static MemberDetailDTO ToDetailDto(this Member aMember, bool aIncludeDiscordOnlyRoles = true)
        {
            IEnumerable<Role> lRoleList = aIncludeDiscordOnlyRoles
                ? aMember.Roles
                : aMember.Roles
                    .Where(role => role.RoleType != RoleTypesEnum.DiscordOnly)
                    .OrderByDescending(role => role.Position);

            var lRoleDTOList = lRoleList.Select(role => role.ToDto()).ToImmutableArray();
            return new MemberDetailDTO(aMember.DiscordGuildDisplayName, aMember.DiscordAvatarUrl, aMember.GameHandle, aMember.SpectrumCommunityMoniker, aMember.IsGameHandleVerified, aMember.Status, lRoleDTOList);
        }
    }
}
