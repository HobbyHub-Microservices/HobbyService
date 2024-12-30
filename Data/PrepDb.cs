using HobbyService.Models;
using HobbyService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace HobbyService.Data;

public static class PrepDb
{
      public static void PrepPopulation(IApplicationBuilder app)
      {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                  // var grpcClient = serviceScope.ServiceProvider.GetService<IUserDataClient>();
                  // var users = grpcClient.ReturnAllUsers();
                  
                  SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>());
            }
            
           
      }

      private static void SeedData(AppDbContext context)
      {
            // Console.WriteLine("--> Seeding data...");
            // if (users != null)
            // {
            //       foreach (var user in users)
            //       {
            //             if (!repo.ExternalUserExists(user.ExternalId))
            //             {
            //                   repo.CreateUser(user);
            //             }
            //             repo.SaveChanges();
            //       }  
            // }
            //
            // else
            // {
            //       Console.WriteLine("--> No users found.");
            // }
            
            Console.WriteLine("--> Trying to apply migrations...");
            try
            {
                  context.Database.Migrate();
            }
            catch (Exception ex)
            {
                  Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }

            if (!context.Hobbies.Any())
            {
                  Console.WriteLine("---> Seeding data ...");
                  context.Hobbies.AddRange(
                        new Hobby
                        {
                              Id = 1,
                              Name = "Photography",
                              Description = "Capturing moments through a lens.",
                              CreatedAt = DateTime.UtcNow
                        },
                        new Hobby
                        {
                              Id = 2,
                              Name = "Gardening",
                              Description = "Growing and nurturing plants.",
                              CreatedAt = DateTime.UtcNow
                        },
                        new Hobby
                        {
                              Id = 3,
                              Name = "Cooking",
                              Description = "Creating delicious meals.",
                              CreatedAt = DateTime.UtcNow
                        },
                        new Hobby
                        {
                              Id = 4,
                              Name = "Hiking",
                              Description = "Exploring trails and nature.",
                              CreatedAt = DateTime.UtcNow
                        },
                        new Hobby
                        {
                              Id = 5,
                              Name = "Painting",
                              Description = "Expressing creativity on canvas.",
                              CreatedAt = DateTime.UtcNow
                        },
                        new Hobby
                        {
                              Id = 6,
                              Name = "Writing",
                              Description = "Crafting stories and sharing thoughts.",
                              CreatedAt = DateTime.UtcNow
                        },
                        new Hobby
                        {
                              Id = 7,
                              Name = "Fishing",
                              Description = "Relaxing by the water and catching fish.",
                              CreatedAt = DateTime.UtcNow
                        },
                        new Hobby
                        {
                              Id = 8,
                              Name = "Gaming",
                              Description = "Playing video games across various platforms.",
                              CreatedAt = DateTime.UtcNow
                        }
                  
                  );
                  context.SaveChanges();

            }

      }
}