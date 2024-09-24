using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NuGet.Protocol;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Models;
using Xellarium.Shared;

namespace Xellarium.Server;

public static class DataAccessConfiguration
{
    public static async Task EnsureSeedData(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<XellariumContext>();
            var userService = serviceScope.ServiceProvider.GetRequiredService<IUserService>();
            var authenticationService = serviceScope.ServiceProvider.GetRequiredService<IAuthenticationService>();
            var ruleService = serviceScope.ServiceProvider.GetRequiredService<IRuleService>();
            var collectionService = serviceScope.ServiceProvider.GetRequiredService<ICollectionService>();
            var neighborhoodService = serviceScope.ServiceProvider.GetRequiredService<INeighborhoodService>();

            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }

            if (!context.Users.Any())
            {
                var moore = await neighborhoodService.AddNeighborhood(new Neighborhood()
                {
                    Id = 1,
                    Name = "Окрестность Мура",
                    Offsets = Neighborhood.MooreOffsets
                });
                
                var user = await authenticationService.RegisterUser("admin", "admin");
                user.Role = UserRole.Admin;
                
                var col = new Collection()
                {
                    Id = 1,
                    Name = "Admin collection"
                };
                await userService.AddCollection(user.Id, col);
                
                var golRule = new Rule()
                {
                    Id = 1,
                    GenericRule = GenericRule.GameOfLife,
                    Name = "Game of Life",
                    NeighborhoodId = moore.Id
                };
                var wireworldRule = new Rule()
                {
                    Id = 2,
                    GenericRule = GenericRule.WireWorld,
                    Name = "WireWorld",
                    NeighborhoodId = moore.Id
                };
                var addedGol = (await userService.AddRule(user.Id, golRule))!;
                await userService.AddToCollection(col.Id, addedGol);
                var addedWireworld = (await userService.AddRule(user.Id, wireworldRule))!;
                await userService.AddToCollection(col.Id, addedWireworld);
                await userService.UpdateUser(user);
            }
        }
    }
    
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var usedDatabase = builder.Configuration["UsedDatabase"];
        var dbConnectConfig = builder.Configuration.GetRequiredSection("Databases").GetRequiredSection(usedDatabase!);
        if (usedDatabase == "Postgres")
        {
            var connectionString = dbConnectConfig["ConnectionString"];
            builder.Services.AddDbContext<XellariumContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(connectionString));
        }
        else if (usedDatabase == "InMemory")
        {
            var databaseName = dbConnectConfig["DatabaseName"]!;
            builder.Services.AddDbContext<XellariumContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseInMemoryDatabase(databaseName));
        }
        else
        {
            throw new Exception("Unknown database type");
        }
    }
}