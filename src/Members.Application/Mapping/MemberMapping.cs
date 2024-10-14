using Common.Application.DTOs.Members;
using Common.Application.DTOs.Roles;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using System.Collections.Immutable;

namespace Members.Application.Mapping
{
    public static class MemberMapping
    {
        public static MemberDTO ToDto(this Member aMember, IEnumerable<Role> roles)
        {
            var lRoleDTOList = roles
                .Select(role => role.ToDto())
                .ToImmutableArray();
            return new MemberDTO(aMember.Id, aMember.GuildId.ToString(), aMember.DiscordGuildDisplayName, aMember.GameHandle, aMember.Status, lRoleDTOList);
        }
        public static MemberDTO ToDto(this Member aMember)
        => aMember.ToDto([]);
        public static MemberDetailDTO ToDetailDto(this Member aMember, IEnumerable<Role> roles, bool aIncludeDiscordOnlyRoles = true)
        {
            IEnumerable<Role> lRoleList = aIncludeDiscordOnlyRoles
                ? roles
                : roles
                    .Where(role => role.RoleType != RoleTypesEnum.DiscordOnly)
                    .OrderByDescending(role => role.Position);

            var lRoleDTOList = lRoleList.Select(role => role.ToDto()).ToImmutableArray();
            return new MemberDetailDTO(aMember.Id, aMember.GuildId.ToString(), aMember.DiscordGuildDisplayName, aMember.DiscordAvatarUrl, aMember.GameHandle, aMember.SpectrumCommunityMoniker, aMember.IsGameHandleVerified, aMember.Status, lRoleDTOList);
        }
        public static MemberDetailDTO ToDetailDto(this Member aMember, bool aIncludeDiscordOnlyRoles = true)
        => aMember.ToDetailDto([]);
    }
}
