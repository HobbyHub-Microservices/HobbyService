using HobbyService.Models;
using HobbyService.SyncDataServices.Grpc;

namespace HobbyService.Data;

public static class PrepDb
{
      public static void PrepPopulation(IApplicationBuilder app)
      {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                  var grpcClient = serviceScope.ServiceProvider.GetService<IUserDataClient>();
                  var users = grpcClient.ReturnAllUsers();
                  
                  SeedData(serviceScope.ServiceProvider.GetService<IHobbyRepo>(), users);
            }
            
           
      }

      private static void SeedData(IHobbyRepo repo, IEnumerable<User> users)
      {
            Console.WriteLine("--> Seeding data...");
            if (users != null)
            {
                  foreach (var user in users)
                  {
                        if (!repo.ExternalUserExists(user.ExternalId))
                        {
                              repo.CreateUser(user);
                        }
                        repo.SaveChanges();
                  }  
            }
            else
            {
                  Console.WriteLine("--> No users found.");
            }


      }
}