using FluentValidation;
using Members.Domain.Errors;
using TGF.Common.ROP.Errors;

namespace Members.Domain.Validation
{
    public class MemberVerificationValidator : AbstractValidator<Entities.Member>
    {
        public MemberVerificationValidator()
        {
            RuleFor(member => !member.IsGameHandleVerified)
                .Must(isVerified => isVerified == false)
                .WithROPError(DomainErrors.Validation.Member.AlreadyVerified);

            RuleFor(member => member.VerifyCode)
                .NotNull()
                .WithROPError(DomainErrors.Validation.VerifyCode.Null)
                .DependentRules(() =>
                {
                    RuleFor(member => member.VerifyCode!.ExpiryDate)
                        .Must(expiryDate => expiryDate > DateTimeOffset.Now)
                        .WithROPError(DomainErrors.Validation.VerifyCode.Expired);
                });
        }
    }
}
