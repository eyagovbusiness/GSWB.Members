﻿using Common.Domain.ValueObjects;

namespace Members.Domain.ValueObjects
{
    public record class MemberStatus(MemberKey Id, MemberStatusEnum Status);
}