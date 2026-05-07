using Microsoft.AspNetCore.Mvc;

namespace BackendNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("error")]
        public IActionResult GetError()
        {
            throw new Exception("Test Error: Serilog și Middleware funcționează!");
        }
    }
}