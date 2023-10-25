using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IServiceProvider serviceProvider, bool isProduction)
        {
           
            using(var serviceScope=  serviceProvider.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {
             if(isProduction)
            {
                try
                {
                    Console.WriteLine("----> Attempting to apply migrations....");
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"----> could not run migrations : {ex.Message}");
                }
            }
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