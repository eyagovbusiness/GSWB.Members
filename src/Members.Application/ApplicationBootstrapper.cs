using Common.Application;
using FluentValidation;
using Members.Application.Services;
using Members.Application.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TGF.CA.Application.UseCases;

namespace Members.Application
{
    /// <summary>
    /// Provides methods for configuring and using the application specific services.
    /// </summary>
    public static class ApplicationBootstrapper
    {
        /// <summary>
        /// Configures the specific required services for this web application.
        /// </summary>
        /// <param name="aServiceList"></param>
        public static void RegisterApplicationServices(this IServiceCollection aServiceList)
        {
            aServiceList.RegisterCommonApplicationServices();
            aServiceList.AddScoped<IMembersService, MembersService>();
            aServiceList.AddValidatorsFromAssemblyContaining<RoleSortingValidator>();
            aServiceList.AddUseCases(Assembly.GetExecutingAssembly());
        }
    }
}
