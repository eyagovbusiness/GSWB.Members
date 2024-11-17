using Common.Domain.ValueObjects;

namespace Members.Domain.ValueObjects
{
    public record MemberGameProfileUpdate(
        MemberKey Id, 
        string? GameHandle,
        string? SpectrumCommunityMoniker
    );
}
