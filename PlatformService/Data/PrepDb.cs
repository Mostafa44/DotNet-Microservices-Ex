using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IServiceProvider serviceProvider)
        {
            using(var serviceScope=  serviceProvider.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
            if(!context.Platforms.Any())
            {
                Console.WriteLine("---> Seeding some data.....");
                context.Platforms.AddRange(
                    new Platform()
                    {
                        Name="dotent",
                        Publisher="Microsoft",
                        Cost= "Free"
                    },
                     new Platform()
                    {
                        Name="Sql-Server",
                        Publisher="Microsoft",
                        Cost= "Free"
                    },
                     new Platform()
                    {
                        Name="Kubernetes",
                        Publisher="Cloud Native Computing Foundation",
                        Cost= "Free"
                    }

                );
                context.SaveChanges();

            }
            else
            {
                Console.WriteLine("---> we already have data");
            }
            
        }
    }
}