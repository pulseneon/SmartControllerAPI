using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_db.Controllers
{
    [Route("")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public IActionResult Get()
        {
            return Redirect("/swagger/index.html");
        }
    }
}
