using Common.Presentation;
using FluentValidation;
using HealthChecks.UI.Client;
using Members.Application.Validation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Reflection;
using TGF.CA.Application;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;

namespace Members.API
{
    /// <summary>
    /// Provides methods for configuring and using the application specific presentation layer components.
    /// </summary>
    public static class PresentationBootstrapper
    {
        /// <summary>
        /// Make the configurations at the presentation layer for this web application.
        /// </summary>
        public static void ConfigurePresentation(this WebApplicationBuilder aWebApplicationBuilder)
        {
            aWebApplicationBuilder.ConfigureCommonPresentation();

            var lXmlDocFileName = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var lXmlDocFilePath = Path.Combine(AppContext.BaseDirectory, lXmlDocFileName);

            aWebApplicationBuilder.ConfigureDefaultPresentation(
                [lXmlDocFilePath],
                aBaseSwaggerPath: "members-ms",
                aScanMarkerList: typeof(PresentationErrors)
            );
            aWebApplicationBuilder.Services.AddValidatorsFromAssemblyContaining<MembersSortingValidator>();
        }

        /// <summary>
        /// Applies the presentation configurations to the web application and setup the middleware pipeline
        /// </summary>
        public static void UsePresentation(this WebApplication aWebApplication)
        {
            aWebApplication.UseCommonPresentation();

            aWebApplication.UseCustomErrorHandlingMiddleware();
            aWebApplication.UseRouting();//UseRouting() must be called before UseAuthentication() and UseAuthorization() which is UseIdentity().
            aWebApplication.UseIdentity();

            aWebApplication.UseEndpointDefinitions();
        }

    }
}
