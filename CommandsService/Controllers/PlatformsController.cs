using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController :ControllerBase
    {
     public PlatformsController()
     {
        
     }   

    [HttpPost]
     public ActionResult TestInBoundConnections()
     {
        Console.WriteLine("---> In Bound Post # command Service");
        return Ok("Inbound Test of from Inbound controller");
     }
    }
}