using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFatory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFatory,
                              IMapper mapper)
        {
            _scopeFatory = scopeFatory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType= DetermineEvent(message);
            switch(eventType)
            {
                case EventType.PlatformPublished:
                    //do something
                    break;
                default:
                    break;
            }
        }
        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("---> Determining Event");
            var eventType= JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch(eventType.Event)
            {
                case "Platform_Published":
                  Console.WriteLine("---> Platform Published Event Detected");
                  return EventType.PlatformPublished;
                default:
                    Console.WriteLine("---> Could not determine the Event Type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform (string platformPublishedMessage)
        {
           using(var scope = _scopeFatory.CreateScope())
           {
                var repo= scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto= JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat= _mapper.Map<Platform>(platformPublishedDto);
                    if(! repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                    }
                    else
                    {
                        Console.WriteLine(" ---> Platform already exists...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"---> Could not add Platform to DB {ex.Message} ");
                    
                }
           }
        }
    }

    enum EventType
    {
        PlatformPublished ,
        Undetermined
    }
}