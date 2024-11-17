using Common.Domain.ValueObjects;

namespace Members.Domain.ValueObjects
{
    public record class MemberDiscordDisplayName(MemberKey Id, string DisplayName);
}
