using Common.Domain.Validation;
using FluentValidation;
using Members.Domain.Services;
using Members.Domain.Validation.Guild;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Members.Domain
{
    /// <summary>
    /// Provides methods for configuring and using the common domain components shared across several web application projects in this solution.
    /// </summary>
    public static class DomainBootstrapper
    {
        /// <summary>
        /// Configures the necessary common domain services for the application shared across several web application projects in this solution.
        /// </summary>
        public static WebApplicationBuilder ConfigureDomain(this WebApplicationBuilder aWebApplicationBuilder)
        {
            aWebApplicationBuilder.Services.AddValidatorsFromAssemblyContaining<GuildRoleIdValidator>();
            aWebApplicationBuilder.Services.AddScoped<GuildMemberRoleService>();
            return aWebApplicationBuilder;
        }

        //public static void UseCommonDomain(this WebApplication aWebApplication)
        //{
        //}
    }
}
