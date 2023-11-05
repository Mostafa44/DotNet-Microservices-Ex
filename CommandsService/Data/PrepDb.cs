using CommandsService.Models;
using CommandsService.SyncDataServices;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrerpPopulation(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope= applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient= serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms= grpcClient.ReturnAllPaltforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo commandRepo, IEnumerable<Platform>platforms)
        {
            Console.WriteLine("---> Seeding new platforms");
            foreach(var platform in platforms)
            {
                if(! commandRepo.ExternalPlatformExists(platform.ExternalId))
                {
                    commandRepo.CreatePlatform(platform);
                }
                commandRepo.SaveChanges();
            }
        }
    }
}