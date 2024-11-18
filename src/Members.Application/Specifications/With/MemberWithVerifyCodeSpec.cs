using Ardalis.Specification;
using Members.Domain.Entities;

namespace Members.Application.Specifications.With
{
    public class MemberWithVerifyCodeSpec : Specification<Member>
    {
        public MemberWithVerifyCodeSpec() {
            Query.Include(x => x.VerifyCode);
        }
    }
}
