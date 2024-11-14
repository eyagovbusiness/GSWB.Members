using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Domain.Entities;
using System.Collections.Immutable;

namespace Members.Application.Mapping
{
    public static class MemberMapping
    {
        public static MemberDTO ToDto(this Member aMember)
        => new(
            aMember.UserId.ToString(),
            aMember.GuildId.ToString(),
            aMember.DiscordGuildDisplayName,
            aMember.DiscordAvatarUrl,
            aMember.GameHandle,
            aMember.Status,
            aMember.Roles.Select(memberRole => memberRole.RoleId.ToString())
                .ToImmutableArray()
        );
        public static MemberDetailDTO ToDetailDto(this Member aMember, IEnumerable<Role> roles, bool aIncludeDiscordOnlyRoles = true)
        {
            IEnumerable<Role> lRoleList = aIncludeDiscordOnlyRoles
                ? roles
                : roles
                    .Where(role => role.RoleType != RoleTypesEnum.DiscordOnly)
                    .OrderByDescending(role => role.Position);

            var lRoleDTOList = lRoleList.Select(role => role.ToDto()).ToImmutableArray();
            return new MemberDetailDTO(aMember.UserId.ToString(), aMember.GuildId.ToString(), aMember.DiscordGuildDisplayName, aMember.DiscordAvatarUrl, aMember.GameHandle, aMember.SpectrumCommunityMoniker, aMember.IsGameHandleVerified, aMember.Status, lRoleDTOList);
        }

        public static MemberDetailDTO ToDetailDto(this Member aMember, bool aIncludeDiscordOnlyRoles = true)
        => aMember.ToDetailDto([], aIncludeDiscordOnlyRoles);
    }
}
