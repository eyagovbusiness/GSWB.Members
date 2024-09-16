using Common.Application.Contracts.Services;
using Common.Infrastructure;
using Common.Infrastructure.Communication.HTTP;
using Members.Application;
using Members.Domain.Contracts.Repositories;
using Members.Infrastructure.Communication.MessageConsumer.Member;
using Members.Infrastructure.Repositories;
using Members.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure;
using TGF.CA.Infrastructure.Communication.RabbitMQ;
using TGF.CA.Infrastructure.DB.PostgreSQL;

namespace Members.Infrastructure
{
    /// <summary>
    /// Provides methods for configuring and using the application specific infrastructure layer components.
    /// </summary>
    public static class InfrastructureBootstrapper
    {

        /// <summary>
        /// Configures the necessary infrastructure services for the application.
        /// </summary>
        /// <param name="aWebApplicationBuilder">The web application builder.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task ConfigureInfrastructureAsync(this WebApplicationBuilder aWebApplicationBuilder)
        {
            await aWebApplicationBuilder.ConfigureCommonInfrastructureAsync();

            await aWebApplicationBuilder.Services.AddPostgreSQL<MembersDbContext>("MembersDb");
            aWebApplicationBuilder.Services.AddScoped<IRoleRepository, RoleRepository>();
            aWebApplicationBuilder.Services.AddScoped<IMemberRepository, MemberRepository>();
            aWebApplicationBuilder.Services.AddHostedService<StartupHostedService>();

            aWebApplicationBuilder.Services.AddHttpClient();
            aWebApplicationBuilder.Services.AddScoped<IRolesInfrastructureService, RolesInfrastructureService>();
            aWebApplicationBuilder.Services.AddScoped<IGameVerificationService, GameVerificationService>();
            aWebApplicationBuilder.ConfigureCommunication();

        }

        /// <summary>
        /// Configure the communication-related services such as message broker related services or direct communication related services
        /// </summary>
        public static void ConfigureCommunication(this WebApplicationBuilder aWebApplicationBuilder)
        {
            aWebApplicationBuilder.Services.AddScoped<ISwarmBotCommunicationService, SwarmBotCommunicationService>();

            aWebApplicationBuilder.Services.AddServiceBusIntegrationPublisher();

            aWebApplicationBuilder.Services.AddMessageHandlersInAssembly<MemberRenameHandler>();
            aWebApplicationBuilder.Services.AddServiceBusIntegrationConsumer();
        }

        /// <summary>
        /// Applies the infrastructure configurations to the web application.
        /// </summary>
        /// <param name="aWebApplication">The Web application instance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task UseInfrastructure(this WebApplication aWebApplication)
        {
            aWebApplication.UseCommonInfrastructure();
            await aWebApplication.UseMigrations<MembersDbContext>();
        }

    }
}
