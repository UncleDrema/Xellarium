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
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<XellariumContext>>();
            
            logger.LogInformation("Getting context and services");
            var context = serviceScope.ServiceProvider.GetRequiredService<XellariumContext>();
            var userService = serviceScope.ServiceProvider.GetRequiredService<IUserService>();
            var authenticationService = serviceScope.ServiceProvider.GetRequiredService<IAuthenticationService>();
            var ruleService = serviceScope.ServiceProvider.GetRequiredService<IRuleService>();
            var collectionService = serviceScope.ServiceProvider.GetRequiredService<ICollectionService>();
            var neighborhoodService = serviceScope.ServiceProvider.GetRequiredService<INeighborhoodService>();

            if (context.Database.IsRelational())
            {
                logger.LogInformation("Migrating database");
                await context.Database.MigrateAsync();
            }
            
            if (!context.Users.Any())
            {
                logger.LogInformation("Seeding data");
                
                logger.LogInformation("Adding neighborhoods");
                var moore = new Neighborhood()
                {
                    Name = "Окрестность Мура",
                    Offsets = Neighborhood.MooreOffsets
                };
                await neighborhoodService.AddNeighborhood(moore);
                logger.LogInformation("Added neighborhood {Name} id={Id}", moore.Name, moore.Id);
                
                logger.LogInformation("Adding admin user");
                var user = await authenticationService.RegisterUser("admin", "admin");
                logger.LogInformation("Added user {Name} id={Id}", user.Name, user.Id);
                logger.LogInformation("Granting admin user admin role");
                user.Role = UserRole.Admin;
                await userService.UpdateUser(user);
                
                logger.LogInformation("Adding collection to admin user");
                var col = new Collection()
                {
                    Name = "Admin collection"
                };
                await userService.AddCollection(user.Id, col);
                logger.LogInformation("Added collection {Name} id={Id}", col.Name, col.Id);
                
                logger.LogInformation("Adding rules to admin collection");
                logger.LogInformation("Adding Game of Life rule");
                var golRule = new Rule()
                {
                    GenericRule = GenericRule.GameOfLife,
                    Name = "Game of Life",
                    Neighborhood = moore
                };
                await userService.AddRule(user.Id, golRule);
                logger.LogInformation("Added rule {Name} id={Id}", golRule.Name, golRule.Id);
                
                logger.LogInformation("Adding WireWorld rule");
                var wireworldRule = new Rule()
                {
                    GenericRule = GenericRule.WireWorld,
                    Name = "WireWorld",
                    Neighborhood = moore
                };
                await userService.AddRule(user.Id, wireworldRule);
                logger.LogInformation("Added rule {Name} id={Id}", wireworldRule.Name, wireworldRule.Id);
                
                logger.LogInformation("Adding rules to admin collection");
                await collectionService.AddRule(col.Id, golRule);
                await collectionService.AddRule(col.Id, wireworldRule);
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
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(connectionString));
        }
        else if (usedDatabase == "InMemory")
        {
            var databaseName = dbConnectConfig["DatabaseName"]!;
            builder.Services.AddDbContext<XellariumContext>(options =>
                options
                    .EnableSensitiveDataLogging()
                    .UseInMemoryDatabase(databaseName));
        }
        else
        {
            throw new Exception("Unknown database type");
        }
    }
}