using Members.Application.Services;
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
            aServiceList.AddScoped<IRolesService, RolesService>();
            aServiceList.AddScoped<IMembersService, MembersService>();
            aServiceList.AddUseCases(Assembly.GetExecutingAssembly());
        }
    }
}
