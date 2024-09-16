using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Members.Domain.Validation;
using Members.Domain.Contracts.Services;
using Members.Domain.Services;

namespace Members.Domain
{
    /// <summary>
    /// Provides methods for configuring and using the domain layer specific services.
    /// </summary>
    public static class DomainBootstrapper
    {
        /// <summary>
        /// Configures the specific domain layer required services for this web application.
        /// </summary>
        /// <param name="aServiceList"></param>
        public static void RegisterDomainServices(this IServiceCollection aServiceList)
        {
            aServiceList.AddValidatorsFromAssemblyContaining<MemberVerificationValidator>();
            aServiceList.AddScoped<IMemberRoleService, MemberRoleService>();
        }
    }
}
