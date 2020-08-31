using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Supervisor.Controllers
{
    [ApiController]
    [Route("Automation")]
    public class AutomationWebhookController : ControllerBase
    {
        [Route("Webhook")]
        [HttpPost()]
        public IActionResult ReceiveWebhook()
        {
            return Ok();
        }
    }
}
