using Microsoft.AspNetCore.Mvc;

namespace App.unittests.Controllers
{
    [Route("api/[controller]")]
    public class HttpStatController : ControllerBase
    {
        public IActionResult Get(int code)
        {
            return StatusCode(code);
        }
    }
}
