using Common.Domain;
using Common.Presentation;
using Members.API;
using Members.Application;
using Members.Infrastructure;


WebApplicationBuilder lMembersApplicationBuilder = WebApplication.CreateBuilder(args);

lMembersApplicationBuilder.ConfigureCommonDomain();
await lMembersApplicationBuilder.ConfigureInfrastructureAsync();
lMembersApplicationBuilder.Services.RegisterApplicationServices();
lMembersApplicationBuilder.ConfigureCommonPresentation();
lMembersApplicationBuilder.ConfigurePresentation();

var lMembersWebApplication = lMembersApplicationBuilder.Build();

await lMembersWebApplication.UseInfrastructure();
lMembersWebApplication.UsePresentation();

await lMembersWebApplication.RunAsync();

