using FluentValidation;
using Members.Domain.Entities;
using System.Text.RegularExpressions;
using TGF.Common.ROP.Errors;

namespace Members.Domain.Validation.Member
{
    public class MemberVerifyCodeValidor : AbstractValidator<Entities.Member>
    {
        public MemberVerifyCodeValidor()
        {
            // Set class-level cascade mode to stop on the first failure across all rules
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(member => member.IsGameHandleVerified)
                .NotEqual(true)
                .WithROPError(Errors.Validation.DomainErrors.Validation.Member.GamehandleAlreadyVerified);

            RuleFor(member => member.GameHandle)
                .NotEmpty()
                .WithROPError(Errors.Validation.DomainErrors.Validation.Member.GameHandleNotSet);

            // Rule-level cascade for VerifyCode
            RuleFor(member => member.VerifyCode!)
                .Cascade(CascadeMode.Continue)
                .NotNull()
                .When(member => !string.IsNullOrWhiteSpace(member.GameHandle))
                .Must(BeAValidCode)
                .WithROPError(Errors.Validation.DomainErrors.Validation.Member.VerifyCodeNotValidFormat)
                .Must(BeNotExpired)
                .WithROPError(Errors.Validation.DomainErrors.Validation.Member.VerifyCodeExpired);
        }

        private bool BeAValidCode(VerifyCode verifyCode)
            => Regex.IsMatch(verifyCode.Code, $@"^\d{{{InvariantConstants.VerifyCodeLength}}}$");

        private bool BeNotExpired(VerifyCode verifyCode)
            => verifyCode.ExpiryDate > DateTimeOffset.Now;
    }
}
