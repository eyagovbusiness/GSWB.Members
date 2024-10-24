using Common.Domain;
using Common.Presentation;
using Members.API;
using Members.Application;
using Members.Domain;
using Members.Infrastructure;


WebApplicationBuilder lMembersApplicationBuilder = WebApplication.CreateBuilder(args);

lMembersApplicationBuilder.ConfigureDomain();
await lMembersApplicationBuilder.ConfigureInfrastructureAsync();
lMembersApplicationBuilder.Services.RegisterApplicationServices();
lMembersApplicationBuilder.ConfigurePresentation();

var lMembersWebApplication = lMembersApplicationBuilder.Build();

await lMembersWebApplication.UseInfrastructure();
lMembersWebApplication.UsePresentation();

await lMembersWebApplication.RunAsync();

