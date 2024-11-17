using Common.Domain.ValueObjects;

namespace Members.Domain.ValueObjects
{
    public record class MemberAvatar(MemberKey Id, string AvatarUrl);
}
