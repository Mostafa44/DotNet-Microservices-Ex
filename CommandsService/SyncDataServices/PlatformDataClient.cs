using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using PlatformService;

namespace CommandsService.SyncDataServices
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IOptions<GrpcPlatformOptions> _grpcPlatformOptions;
        private readonly IMapper _mapper;

        public PlatformDataClient(IOptions<GrpcPlatformOptions> grpcPlatformOptions,
                                  IMapper mapper)
        {
            _grpcPlatformOptions = grpcPlatformOptions;
            _mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPaltforms()
        {
            Console.WriteLine($"---> Calling GRPC Service {_grpcPlatformOptions.Value.Platform}");
            var channel= GrpcChannel.ForAddress(_grpcPlatformOptions.Value.Platform);
            var client= new GrpcPlatform.GrpcPlatformClient(channel);
            var request= new GetAllRequest();
            try
            {
                var reply= client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"---> Could not call GRPC Server {ex.Message}");
                return null;
            }
        }
    }
}